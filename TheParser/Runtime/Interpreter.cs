using System.Diagnostics;
using TheParser.Syntax;
using TheParser.Lexing;
using TheParser.Runtime.Functions;
using TheParser.Runtime.Exceptions;
using TheParser.DependencyInjection;

namespace TheParser.Runtime;

public class Interpreter
{
    private Dictionary<string, Interpretation> _variables = new();
    private Dictionary<string, IFunction> _functions;

    public Interpreter(DependencyInjector injector)
    {
        _functions = BuiltInFunctionScanner.ScanAndCreateBuiltInFunctions(injector);
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
                 new NullInterpretation();

                _variables.Add((string)vds.Identifier.Value!, interp);
                break;
        }
    }

    public Interpretation InterpretExpression(Expr expr)
    {
        switch (expr)
        {
            case CallExpression ce:
                return InvokeFunction(ce);
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
                if (!_variables.TryGetValue(ie.Identifier, out var interp))
                    throw new UnresolvedVariableException(ie.Identifier, ie.Span);
                return interp;
            default:
                throw new NotImplementedException($"Interpretation of expression `{expr.GetType().Name}` is not implemented.");
        }
    }

    public Interpretation InvokeFunction(CallExpression ce)
    {
        if (ce.Callee is not IdentifierExpression functionIdent)
            throw new NotImplementedException();

        string identString = functionIdent.Identifier;

        if (!_functions.TryGetValue(identString, out IFunction? func))
            throw new UnresolvedFunctionException(identString, functionIdent.Span);

        var parameters = func.GetParameterTypes();

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

    private Interpretation SolveBinaryOperation(Interpretation left, TokenType @operator, Interpretation right, SourceSpan span)
    {
        Debug.Assert(TokenUtility.IsOperator(@operator));

        switch (left)
        {
            case NumberInterpretation leftNumber when right is NumberInterpretation rightNumber:
                double value = Calculate(leftNumber.Value, @operator, rightNumber.Value);
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

    private static double Calculate(double left, TokenType @operator, double right)
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

