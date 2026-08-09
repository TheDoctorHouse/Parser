using System.Diagnostics;
using TheParser.Syntax;
using TheParser.Lexing;

namespace TheParser.Runtime;

public interface IStringInterpretable
{
    StringInterpretation InterpretToString();
}

public abstract record class Interpretation;

public record class NothingInterpretation : Interpretation, IStringInterpretable
{
    public StringInterpretation InterpretToString()
    {
        return new StringInterpretation("Nothing.");
    }
}

public record class StringInterpretation(string Value) : Interpretation, IStringInterpretable
{
    public StringInterpretation InterpretToString()
    {
        return new StringInterpretation(Value);
    }
}

public record class NumberInterpretation(double Value) : Interpretation, IStringInterpretable
{
    public StringInterpretation InterpretToString()
    {
        return new StringInterpretation(Value.ToString());
    }
}

public record class NullInterpretation : Interpretation;

public class Interpreter
{
    private Dictionary<string, Interpretation> _variables = new();

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
                var interp = vds.initializer != null ?
                 InterpretExpression(vds.initializer) : 
                 new NullInterpretation();

                _variables.Add((string)vds.identifier.Value!, interp);
                break;
        }
    }

    private Interpretation InterpretExpression(Expr expr)
    {
        switch(expr)
        {
            case CallExpression ce:
                if (ce.Callee is not IdentifierExpression functionIdent) 
                    throw new NotImplementedException();

                string identString = functionIdent.Identifier;

                switch (identString) // hardcoded for now
                {
                    case "Print":
                        if (ce.Arguments.Count != 1)
                            throw new InterpretationException($"Expected 1 argument, got {ce.Arguments.Count}");
                        
                        Interpretation interpretation = InterpretExpression(ce.Arguments[0]);

                        string output;
                        if (interpretation is not IStringInterpretable si)
                            output = $"[Interpretation of type {interpretation.GetType().Name}]";
                        else
                            output = si.InterpretToString().Value;

                        Console.Write(output);

                        return new NothingInterpretation();
                    case "Ask":
                        if (ce.Arguments.Count != 0)    
                            throw new InterpretationException($"Expected 0 arguments, got {ce.Arguments.Count}");
                        
                        string input = Console.ReadLine() ?? "Nothing.";
                        return new StringInterpretation(input);
                    case "ConvertToNumber":
                        if (ce.Arguments.Count != 1)    
                            throw new InterpretationException($"Expected 1 argument, got {ce.Arguments.Count}");

                        Interpretation stringInterp = InterpretExpression(ce.Arguments[0]);
                        if (stringInterp is not StringInterpretation se)
                            throw new InterpretationException($"Expected string interpretation, got {ce.Arguments[0].GetType().FullName}");

                        if (!double.TryParse(se.Value, out double val))
                            throw new InterpretationException($"Failed to parse integer.");

                        return new NumberInterpretation(val);
                    default:
                        throw new InterpretationException($"Cannot resolve function call `{identString}`");
                }
            case StringExpression se:
                return new StringInterpretation(se.Value);
            case NumberExpression ne:
                return new NumberInterpretation(ne.Value);
            case BinaryExpression be:
                return SolveBinaryOperation(InterpretExpression(be.Left), be.Operator, InterpretExpression(be.Right));
            case UnaryExpression ue:
                return SolveUnaryOperation(InterpretExpression(ue.Expr), ue.Operator);
            case IdentifierExpression ie:
                if (!_variables.TryGetValue(ie.Identifier, out var interp))
                    throw new InterpretationException($"No such declaration `{ie.Identifier}`");
                return interp;
            default:
                throw new NotImplementedException($"Interpretation of expression `{expr.GetType().Name}` is not implemented.");
        }
    }

    private Interpretation SolveBinaryOperation(Interpretation left, TokenType @operator, Interpretation right)
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
                throw new OperationInterpretationException(left, @operator, right);
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

    private Interpretation SolveUnaryOperation(Interpretation interpretation, TokenType @operator)
    {
        Debug.Assert(TokenUtility.IsOperator(@operator));

        switch (interpretation)
        {
            case NumberInterpretation ni when @operator is TokenType.Plus or TokenType.Minus:
                var result = @operator == TokenType.Plus ? ni.Value : -ni.Value;

                return new NumberInterpretation(result);
            default:
                throw new OperationInterpretationException(interpretation, @operator);
        }
    }

    private void InterpretExpressionStatement(ExpressionStatement es)
    {
        throw new NotImplementedException();
    }
}

public class InterpretationException : Exception
{
    public InterpretationException(string message) : base(message) { }
}

public class OperationInterpretationException : InterpretationException
{
    public OperationInterpretationException(Interpretation left, TokenType @operator, Interpretation right) :
     base($"Cannot solve binary operation `{left.GetType().Name} {@operator} {right.GetType().Name}") {}

    public OperationInterpretationException(Interpretation left, TokenType @operator) :
     base($"Cannot solve unary operation `{@operator} {left.GetType().Name}") {}
}
