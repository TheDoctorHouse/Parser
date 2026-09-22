using System.Diagnostics;
using TheParser.Syntax;
using TheParser.Lexing;
using TheParser.Runtime.Functions;
using TheParser.Runtime.Exceptions;
using TheParser.DependencyInjection;

namespace TheParser.Runtime;

public class Interpreter
{
    private Dictionary<string, Space> _spaces = [];

    private const string BuiltInSpaceName = "env";
    private const string DefaultSpaceName = "free";

    private Space _currentSpace;

    public Interpreter(DependencyInjector injector)
    {
        var functions = BuiltInFunctionScanner.ScanAndCreateBuiltInFunctions(injector);
        var buildInSpace = new Space(BuiltInSpaceName, [], functions);
        _spaces.Add(buildInSpace.Name, buildInSpace);

        var defaultSpace = new Space(DefaultSpaceName, [], []);
        _currentSpace = defaultSpace;
        _spaces.Add(_currentSpace.Name, _currentSpace);
    }

    public void InterpretStatement(Statement statement)
    {
        switch (statement)
        {
            case BlockStatement bs:
                foreach (var st in bs.Statements)
                    InterpretStatement(st);
                break;
            case ExpressionStatement es:
                InterpretExpression(es.Callee);
                break;
            case VariableDeclarationStatement vds:
                var interp = vds.Initializer != null ?
                 InterpretExpression(vds.Initializer) :
                 new NothingInterpretation();
                string varName = (string)vds.Identifier.Value!;

                if (!_currentSpace.TryAddVariable(varName, interp))
                    throw new VariableDeclarationException($"Variable `{varName}` is already defined.", vds.Span);

                break;
            case IfStatement @is:
                var conditionResult = InterpretExpression(@is.Condition);
                if (conditionResult is not BooleanInterpretation isTrue)
                    throw new UnexpectedTypeException(
                        $"Expected {nameof(BooleanInterpretation)}, got {conditionResult.GetType().Name}",
                        @is.Condition.Span);

                if (isTrue.Value)
                    InterpretStatement(@is.Then);
                else if (@is.Else is not null)
                    InterpretStatement(@is.Else);
                break;
            case DefineSpaceStatement ds:
                DefineSpace(ds);
                break;
            case UndefineSpaceStatement uss:
                UndefineSpace(uss.Span);
                break;
        }
    }

    public Interpretation InterpretExpression(Expr expr)
    {
        switch (expr)
        {
            case CallExpression ce:
                return InvokeFunction(_currentSpace, ce);
            case StringExpression se:
                return new StringInterpretation(se.Value);
            case NumberExpression ne:
                return new NumberInterpretation(ne.Value);
            case BooleanExpression bne:
                return new BooleanInterpretation(bne.Value);
            case BinaryExpression be:
                return SolveBinaryOperation(InterpretExpression(be.Left), be.Operator, InterpretExpression(be.Right), be.Span);
            case UnaryExpression ue:
                return SolveUnaryOperation(InterpretExpression(ue.Expr), ue.Operator, ue.Span);
            case IdentifierExpression ie:
                return GetVariableOrFail(_currentSpace, ie);
            case SpaceAccessExpression sa:
                return InterpretAccessExpression(sa);
            default:
                throw new NotImplementedException($"Interpretation of expression `{expr.GetType().Name}` is not implemented.");
        }
    }

    public Interpretation InvokeFunction(Space space, CallExpression ce)
    {
        if (ce.Callee is not IdentifierExpression functionIdent)
            throw new NotImplementedException();

        string identString = functionIdent.Identifier;

        if (!space.TryGetFunction(identString, out IFunction? func))
            throw new UnresolvedFunctionException(identString, functionIdent.Span);

        var parameters = func!.GetParameterTypes();

        if (parameters.Count != ce.Arguments.Count)
            throw new InvalidArgumentsException(
                $"Expected {parameters.Count} argument(s), got {ce.Arguments.Count}.",
                ce.Span);

        List<Interpretation> arguments = [];

        for (int i = 0; i < parameters.Count; i++)
        {
            var requiredType = parameters[i];
            var evaluated = InterpretExpression(ce.Arguments[i]);
            var evaluatedType = evaluated.GetType();

            if (!requiredType.IsAssignableFrom(evaluatedType))
                throw new InvalidArgumentsException($"Expected {requiredType.FullName}, got {evaluatedType.FullName}.", ce.Arguments[i].Span);

            arguments.Add(evaluated);
        }

        try
        {
            return func.Invoke(arguments);
        }
        catch (FunctionException fe)
        {
            throw new FunctionInvocationException("An error occured during function invocation.", ce.Span, fe);
        }
    }

    private void DefineSpace(DefineSpaceStatement defineSpace)
    {
        if (_currentSpace != null && _currentSpace.Name != DefaultSpaceName)
        {
            throw new SpaceDefinitionException(
                $"Space is already defined: {_currentSpace.Name}. Undefine space before defining new.",
                defineSpace.Span);
        }

        var identifier = (string)defineSpace.Identifier.Value!;
        if (!_spaces.TryGetValue(identifier, out _currentSpace!))
        {
            _currentSpace = new Space(identifier);
            _spaces.Add(identifier, _currentSpace);
        }
    }

    private void UndefineSpace(SourceSpan span)
    {
        if (_currentSpace == null)
        {
            throw new SpaceUndefinitionException("Cannot undefine space since it is already undefined.", span);
        }

        _currentSpace = _spaces[DefaultSpaceName];
    }

    private static Interpretation GetVariableOrFail(Space space, IdentifierExpression identifier)
    {
        if (!space.TryGetVariable(identifier.Identifier, out var interp))
            throw new UnresolvedVariableException(identifier.Identifier, identifier.Span);

        return interp!;
    }

    private Interpretation InterpretAccessExpression(SpaceAccessExpression expr)
    {
        if (expr.Callee is not IdentifierExpression identifier)
        {
            throw new SpaceAccessException(
                $"`{expr.Callee.GetType().Name}` as a space access callee is not allowed.",
                expr.Span);
        }

        if (!_spaces.TryGetValue(identifier.Identifier, out Space? space))
        {
            throw new SpaceAccessException(
                $"Space with name `{identifier.Identifier}` is not defined in the current environment.",
                identifier.Span
            );
        }

        return expr.Target switch
        {
            IdentifierExpression ie => GetVariableOrFail(space, ie),
            CallExpression ce => InvokeFunction(space, ce),
            _ => throw new SpaceAccessException(

                $"Cannot use expression `{expr.Target.GetType().FullName}` with space access.", expr.Target.Span)
        };
    }

    private Interpretation SolveBinaryOperation(Interpretation left, TokenType @operator, Interpretation right, SourceSpan span)
    {
        if (left is NothingInterpretation || right is NothingInterpretation)
        {
            throw new OperationInterpretationException(
                "Cannot operate with nothing types.\n" +
                $"Tried to operate with: {left.GetType().Name} and {right.GetType().Name}." +
                "\nDid you forget do add an initializer?",
                span
                );
        }

        Debug.Assert(TokenUtility.IsOperator(@operator));

        if (TokenUtility.IsComparisonOperator(@operator))
            return CalculateComparison(left, @operator, right, span);

        switch (left)
        {
            case NumberInterpretation leftNumber when right is NumberInterpretation rightNumber:
                double value = CalculateDoubles(leftNumber.Value, @operator, rightNumber.Value);
                return new NumberInterpretation(value);
            case IStringInterpretable leftStr
            when right is IStringInterpretable rightStr
            && @operator is TokenType.Plus:
                string res = leftStr.InterpretToString().Value + rightStr.InterpretToString().Value;
                return new StringInterpretation(res);
            default:
                throw new OperationInterpretationException(left, @operator, right, span);
        }
    }

    private static BooleanInterpretation CalculateComparison(
        Interpretation left,
        TokenType comparisonOperator,
        Interpretation right,
        SourceSpan span)
    {
        switch (left)
        {
            case NumberInterpretation lNi when right is NumberInterpretation rNi:
                switch (comparisonOperator)
                {
                    case TokenType.EqualsEquals:
                        return new(lNi.Value == rNi.Value);
                    case TokenType.LessEqual:
                        return new(lNi.Value <= rNi.Value);
                    case TokenType.GreaterEqual:
                        return new(lNi.Value >= rNi.Value);
                    case TokenType.Greater:
                        return new(lNi.Value > rNi.Value);
                    case TokenType.Less:
                        return new(lNi.Value < rNi.Value);
                    case TokenType.NotEqual:
                        return new(lNi.Value != rNi.Value);
                }
                break;
            case BooleanInterpretation lBi when right is BooleanInterpretation rBi:
                switch (comparisonOperator)
                {
                    case TokenType.EqualsEquals:
                        return new(lBi.Value == rBi.Value);
                    case TokenType.NotEqual:
                        return new(lBi.Value != rBi.Value);
                }
                break;
            case StringInterpretation rSi when right is StringInterpretation lSi:
                switch (comparisonOperator)
                {
                    case TokenType.EqualsEquals:
                        return new(lSi.Value == rSi.Value);
                    case TokenType.NotEqual:
                        return new(lSi.Value != rSi.Value);
                }
                break;
        }

        throw new OperationInterpretationException(left, comparisonOperator, right, span);
    }

    private static double CalculateDoubles(double left, TokenType @operator, double right)
    {
        return @operator switch
        {
            TokenType.Plus => left + right,
            TokenType.Minus => left - right,
            TokenType.Multiply => left * right,
            TokenType.Divide => left / right,
            _ => throw new ArgumentOutOfRangeException(nameof(@operator)),
        };
    }

    private Interpretation SolveUnaryOperation(Interpretation interpretation, TokenType @operator, SourceSpan span)
    {
        Debug.Assert(TokenUtility.IsOperator(@operator));

        switch (interpretation)
        {
            case NumberInterpretation ni when @operator is TokenType.Plus or TokenType.Minus:
                var result = @operator == TokenType.Plus ? ni.Value : -ni.Value;

                return new NumberInterpretation(result);
            default:
                throw new OperationInterpretationException(interpretation, @operator, span);
        }
    }
}

