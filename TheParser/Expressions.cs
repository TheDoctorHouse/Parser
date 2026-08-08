
using System.Diagnostics;

namespace TheParser;

public abstract record class Expr;

public interface IPrintableInformator
{
    string GetInformation();
}

public record class BinaryExpression : Expr, IPrintableInformator
{
    public Expr Left { get; }
    public TokenType Operator { get; }
    public Expr Right { get; }

    public BinaryExpression(Expr left, TokenType @operator, Expr right)
    {
        Debug.Assert(TokenUtility.IsOperator(@operator), "Expected operator, got " + @operator + '.');
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public string GetInformation()
    {
        return TokenUtility.OperatorToString(Operator);
    }
}

public record class NumberExpression(double Value) : Expr, IPrintableInformator
{
    public string GetInformation()
    {
        return Value.ToString();
    }
}

public record class StringExpression(string Value) : Expr, IPrintableInformator
{
    public string GetInformation()
    {
        return Value;
    }
}

public record class UnaryExpression(Expr Expr, TokenType Operator) : Expr, IPrintableInformator
{
    public string GetInformation()
    {
        return TokenUtility.OperatorToString(Operator);
    }
}

public record class IdentifierExpression(string Identifier) : Expr, IPrintableInformator
{
    public string GetInformation()
    {
        return Identifier;
    }
}

public record class CallExpression(Expr Callee, IReadOnlyList<Expr> Arguments) : Expr;