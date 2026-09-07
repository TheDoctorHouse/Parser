using TheParser.Cli;
using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Parsing.Exceptions;
using TheParser.Syntax;

namespace TheParser.Tests;

public class ParserTests
{
    public Expr ParseExpression(string content)
    {
        var lexer = new Lexer(content);
        var astBuilder = new Parser(lexer);
        return astBuilder.ParseAddition();
    }

    public Statement ParseStatement(string content)
    {
        var lexer = new Lexer(content);
        var astBuilder = new Parser(lexer);
        return astBuilder.ParseBlockStatement();
    }

    [Fact]
    public void Parse_MultiplicationHasHigherPrecedenceThanAddition()
    {
        var expression = ParseExpression("1 + 2 * 3");

        var add = Assert.IsType<BinaryExpression>(expression);
        Assert.Equal(1, Assert.IsType<NumberExpression>(add.Left).Value);

        var multiply = Assert.IsType<BinaryExpression>(add.Right);
        Assert.Equal(TokenType.Multiply, multiply.Operator);
    }

    [Fact]
    public void Parse_GroupWithoutClosingParenthesis()
    {
        Assert.Throws<UnexpectedTokenException>(() => ParseStatement("(1 + 2;"));
    }
}