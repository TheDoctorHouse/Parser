
using TheParser.Lexing;
using TheParser.Syntax;

using System.Diagnostics;
using TheParser.Parsing.Exceptions;

namespace TheParser.Parsing;

public class Parser(Lexer lexer)
{
    private Token? _previous = null;

    private int CurrentPosition => lexer.Current != null ? lexer.Current.Position : 0;
    public Token Current => lexer.Current ?? throw new InvalidOperationException("Current token is null.");

    public BlockStatement ParseBlockStatement()
    {
        List<Statement> statements = [];

        int start = CurrentPosition;

        while (Peek().TokenType != TokenType.EOF)
        {
            Statement st;
            if (Match(TokenType.Declaration))
                st = ParseVariableDeclaration();
            else
                st = ParseExpressionStatement();
            statements.Add(st);
        }

        return new BlockStatement(statements, new SourceSpan(start, CurrentPosition - start));
    }

    public VariableDeclarationStatement ParseVariableDeclaration()
    {
        int start = CurrentPosition;

        if (!Match(TokenType.Identifier))
            throw UnexpectedToken(start, TokenType.Identifier);

        var identifier = Current;
        Debug.Assert(Current.Value is string);

        if (Match(TokenType.Semicolon))
            return new VariableDeclarationStatement(identifier, null, new SourceSpan(start, CurrentPosition - start));

        ConsumeOrFail(TokenType.Equals, start);

        var expr = ParseExpression();
        ConsumeOrFail(TokenType.Semicolon, start);

        return new VariableDeclarationStatement(identifier, expr, new SourceSpan(start, CurrentPosition - start));
    }

    public ExpressionStatement ParseExpressionStatement()
    {
        int start = CurrentPosition;
        Expr expr = ParseExpression();

        ConsumeOrFail(TokenType.Semicolon, start);

        return new ExpressionStatement(expr, new SourceSpan(start, CurrentPosition - start));
    }

    public Expr ParseExpression()
    {
        int start = CurrentPosition;
        Expr expr = ParseTerm();

        while (Match(TokenType.Plus, TokenType.Minus))
        {
            Token op = Current;

            Expr right = ParseTerm();

            expr = new BinaryExpression(expr, op.TokenType, right, new SourceSpan(start, CurrentPosition - start));
        }

        return expr;
    }

    public Expr ParseTerm()
    {
        int start = CurrentPosition;
        Expr expr = ParseUnary();

        while (Match(TokenType.Multiply, TokenType.Divide))
        {
            Token op = Current;
            Expr right = ParseUnary();

            expr = new BinaryExpression(expr, op.TokenType, right, new SourceSpan(start, CurrentPosition - start));
        }

        return expr;
    }

    public Expr ParseUnary()
    {
        int start = CurrentPosition;
        if (Match(TokenType.Plus, TokenType.Minus))
        {
            var op = Current;
            var operand = ParseUnary();

            return new UnaryExpression(operand, op.TokenType, new SourceSpan(start, CurrentPosition - start));
        }

        return ParseCall();
    }

    private Expr ParseCall()
    {
        int start = CurrentPosition;
        Expr expr = ParsePrimary();

        while (Match(TokenType.OpeningParentheses))
        {
            expr = FinishCall(expr, start);
        }

        return expr;
    }

    private CallExpression FinishCall(Expr callee, int startPosition)
    {
        List<Expr> arguments = new List<Expr>();

        if (Peek().TokenType != TokenType.ClosingParentheses)
        {
            do
            {
                Expr arg = ParseExpression();
                arguments.Add(arg);
            } while (Match(TokenType.Comma));
        }

        ConsumeOrFail(TokenType.ClosingParentheses, startPosition);

        return new CallExpression(callee, arguments, new SourceSpan(startPosition, CurrentPosition - startPosition));
    }

    public Expr ParsePrimary()
    {
        int start = CurrentPosition;

        if (Match(TokenType.Number))
        {
            return new NumberExpression((double)Current.Value!, new SourceSpan(start, CurrentPosition - start));
        }

        if (Match(TokenType.String))
        {
            return new StringExpression((string)Current.Value!, new SourceSpan(start, CurrentPosition - start));
        }

        if (Match(TokenType.Boolean))
        {
            return new BooleanExpression((bool)Current.Value!, new SourceSpan(start, CurrentPosition - start));
        }

        if (Match(TokenType.Identifier))
        {
            return new IdentifierExpression((string)Current.Value!, new SourceSpan(start, CurrentPosition - start));
        }

        if (Match(TokenType.OpeningParentheses))
        {
            Expr expr = ParseExpression();
            Next();
            return expr;
        }

        throw UnexpectedToken(start);
    }


    private UnexpectedTokenException UnexpectedToken(int start, params TokenType[] expected)
    {
        return new UnexpectedTokenException(
            Current.TokenType,
            new SourceSpan(start, CurrentPosition - start),
            expected
        );
    }

    private Token Next()
    {
        _previous = lexer.Current;
        return lexer.NextToken();
    }

    private Token Peek() => lexer.Peek();

    private Token Previous()
    {
        if (_previous == null)
            throw new InvalidOperationException("No previous token available.");

        return _previous;
    }

    private bool Match(params TokenType[] tokenTypes)
    {
        var token = lexer.Peek();
        foreach (var tokenType in tokenTypes)
        {
            if (token.TokenType == tokenType)
            {
                Next();
                return true;
            }
        }

        return false;
    }

    private void ConsumeOrFail(TokenType tokenType, int start)
    {
        if (!Match(tokenType))
            throw UnexpectedToken(start, tokenType);
    }
}
