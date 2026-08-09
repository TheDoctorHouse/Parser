
using TheParser.Lexing;
using TheParser.Syntax;

using System.Diagnostics;

namespace TheParser.Parsing;

public class AstBuilder
{
    private readonly Lexer _lexer;

    private Token? _previous = null;

    public Token Current => _lexer.Current ?? throw new InvalidOperationException("Current token is null.");
    

    public AstBuilder(Lexer lexer)
    {
        _lexer = lexer;
    }

    public BlockStatement ParseBlockStatement()
    {
        List<Statement> statements = new();

        while (Peek().TokenType != TokenType.EOF)
        {
            Statement st;
            if (Match(TokenType.Declaration))
                st = ParseVariableDeclaration();
            else
                st = ParseExpressionStatement();
            statements.Add(st);
        }

        return new BlockStatement(statements);
    }

    public VariableDeclarationStatement ParseVariableDeclaration()
    {
        if (!Match(TokenType.Identifier))
            throw UnexpectedToken(TokenType.Identifier);

        var identifier = Current;
        Debug.Assert(Current.Value is string);

        if (Match(TokenType.Semicolon))
            return new VariableDeclarationStatement(identifier, null);

        ConsumeOrFail(TokenType.Equals);
        
        var expr = ParseExpression();
        ConsumeOrFail(TokenType.Semicolon);

        return new VariableDeclarationStatement(identifier, expr);
    }

    public ExpressionStatement ParseExpressionStatement()
    {
        Expr expr = ParseExpression();

        ConsumeOrFail(TokenType.Semicolon);

        return new ExpressionStatement(expr);
    }

    public Expr ParseExpression()
    {
        Expr expr = ParseTerm();       

        while (Match(TokenType.Plus, TokenType.Minus))
        {
            Token op = Current;

            Debug.Assert(TokenUtility.IsOperator(op.TokenType), "expr is" + expr.ToString() + op.GetDebugInfo(_lexer));

            Expr right = ParseTerm();

            expr = new BinaryExpression(expr, op.TokenType, right);
        }

        return expr;
    }

    public Expr ParseTerm()
    {
        Expr expr = ParseUnary();

        while (Match(TokenType.Multiply, TokenType.Divide))
        {
            Token op = Current;
            Expr right = ParsePrimary();

            expr = new BinaryExpression(expr, op.TokenType, right);
        }

        return expr;
    }

    public Expr ParseUnary()
    {
        if (Match(TokenType.Plus, TokenType.Minus))
        {
            var op = Current;
            var operand = ParseUnary();

            return new UnaryExpression(operand, op.TokenType);
        }

        return ParseCall();
    }

    private Expr ParseCall()
    {
        Expr expr = ParsePrimary();

        while (Match(TokenType.OpeningParentheses))
        {
            expr = FinishCall(expr);
        }

        return expr;
    }

    private CallExpression FinishCall(Expr callee)
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

        ConsumeOrFail(TokenType.ClosingParentheses);

        return new CallExpression(callee, arguments);
    }

    public Expr ParsePrimary()
    {
        if (Match(TokenType.Number))
        {
            return new NumberExpression((double)Current.Value!);
        }

        if (Match(TokenType.String))
        {
            return new StringExpression((string)Current.Value!);
        }

        if (Match(TokenType.Identifier))
        {
            return new IdentifierExpression((string)Current.Value!);
        }

        if (Match(TokenType.OpeningParentheses))
        {
            Expr expr = ParseExpression();
            Next();
            return expr;
        }

        throw UnexpectedToken();
    }


    private UnexpectedTokenException UnexpectedToken(params TokenType[] expected) 
    {
        return new UnexpectedTokenException(
            Current,
            _lexer,
            expected
        );
    }

    private Token Next()
    {
        _previous = _lexer.Current;
        return _lexer.NextToken();
    }

    private Token Peek() => _lexer.Peek();

    private Token Previous()
    {
        if (_previous == null)
            throw new InvalidOperationException("No previous token available.");

        return _previous;
    }

    private bool Match(params TokenType[] tokenTypes)
    {
        var token = _lexer.Peek();
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

    private void ConsumeOrFail(TokenType tokenType)
    {
        if (!Match(tokenType))
            throw UnexpectedToken(tokenType);
    }
}