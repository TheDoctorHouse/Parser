using System.Runtime.CompilerServices;
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

    [Theory]
    [InlineData("(1 + 2;", TokenType.Semicolon)]
    [InlineData("if {}", TokenType.OpeningBrace)]
    [InlineData("if (true) { } else () { }", TokenType.OpeningParentheses)]
    [InlineData("else if (false) { }", TokenType.Else)]
    [InlineData("if () { }", TokenType.OpeningParentheses)]
    [InlineData("defspace;", TokenType.Semicolon)]
    [InlineData("someaccess.", TokenType.Separator)]
    public void ParseStatement_IncorrectInput_ThrowsUnexpectedTokenException(string input, TokenType received)
    {
        var ex = Assert.Throws<UnexpectedTokenException>(() => ParseStatement(input));
        Assert.Equal(received, ex.ReceivedToken);
    }

    [Theory]
    [InlineData("if (true) { }")]
    [InlineData("if (FunctionCall()) { Foo(); Bar(); }")]
    [InlineData("if (true) { if (false) { Foo(); } else { Bar(); } } else { FooBar(); }")]
    [InlineData("@something = Foo();")]
    [InlineData("Some.Chained.Access.Like.This();")]
    public void ParseStatement_ValidInput_ReturnsStatement(string input)
    {
        Assert.NotNull(ParseStatement(input));
    }

    [Theory]
    [InlineData("defspace somespace;", typeof(DefineSpaceStatement))]
    [InlineData("undefspace;", typeof(UndefineSpaceStatement))]
    [InlineData("if (true) { }", typeof(IfStatement))]
    public void ParseStatement_BlockWithSignleStatement_ReturnsCorrectStatementTypes(
        string input,
        Type expectedType)
    {
        var bs = Assert.IsType<BlockStatement>(ParseStatement(input));
        Assert.Single(bs.Statements);
        Assert.IsType(expectedType, bs.Statements[0]);
    }
}