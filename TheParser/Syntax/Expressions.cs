using TheParser.Lexing;
using System.Diagnostics;

namespace TheParser.Syntax;

public abstract record class Expr(SourceSpan Span);

public interface IPrintableInformator
{
    string GetInformation();
}

public record class BinaryExpression : Expr, IPrintableInformator
{
    public Expr Left { get; }
    public TokenType Operator { get; }
    public Expr Right { get; }

    public BinaryExpression(
        Expr left,
        TokenType @operator,
        Expr right,
        SourceSpan span)
         : base(span)
    {
        Debug.Assert(
            TokenUtility.IsOperator(@operator),
            "Expected operator, got " + @operator + '.');

        Left = left;
        Operator = @operator;
        Right = right;
    }

    public string GetInformation()
    {
        return TokenUtility.OperatorToString(Operator);
    }
}

public record class NumberExpression(double Value, SourceSpan Span) : Expr(Span), IPrintableInformator
{
    public string GetInformation()
    {
        return Value.ToString();
    }
}

public record class StringExpression(string Value, SourceSpan Span) : Expr(Span), IPrintableInformator
{
    public string GetInformation()
    {
        return Value;
    }
}

public record class BooleanExpression(bool Value, SourceSpan Span) : Expr(Span), IPrintableInformator
{
    public string GetInformation()
    {
        return Value.ToString();
    }
}

public record class UnaryExpression(Expr Expr, TokenType Operator, SourceSpan Span) : Expr(Span), IPrintableInformator
{
    public string GetInformation()
    {
        return TokenUtility.OperatorToString(Operator);
    }
}

public record class IdentifierExpression(string Identifier, SourceSpan Span) : Expr(Span), IPrintableInformator
{
    public string GetInformation()
    {
        return Identifier;
    }
}

public record class CallExpression(Expr Callee, IReadOnlyList<Expr> Arguments, SourceSpan Span) : Expr(Span);