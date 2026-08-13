using TheParser.Cli;
using TheParser.Lexing;

namespace TheParser.Tests;

public class LexerTests
{
    private static Lexer CreateLexer(string text)
    {
        return new Lexer(new CliCodeStream(text));
    }

    [Theory]
    [InlineData("+", TokenType.Plus)]
    [InlineData("-", TokenType.Minus)]
    [InlineData("*", TokenType.Multiply)]
    [InlineData("/", TokenType.Divide)]
    [InlineData(",", TokenType.Comma)]
    [InlineData(";", TokenType.Semicolon)]
    [InlineData("@", TokenType.Declaration)]
    [InlineData("=", TokenType.Equals)]
    [InlineData("(", TokenType.OpeningParentheses)]
    [InlineData(")", TokenType.ClosingParentheses)]
    public void NextToken_SingleCharacter_ReturnsExpectedToken(string input, TokenType tokenType)
    {
        var lexer = CreateLexer(input);
        Assert.Equal(tokenType, lexer.NextToken().TokenType);
    }

    [Theory]
    [InlineData("something", TokenType.Identifier)]
    [InlineData("SomeCall(\"something\" + 123)\n;", TokenType.Identifier, TokenType.OpeningParentheses, TokenType.String, TokenType.Plus, TokenType.Number, TokenType.ClosingParentheses, TokenType.Semicolon)]
    public void NextToken_MultipleCharacters_ReturnsExpectedTokens(string input, params TokenType[] expectedTokens)
    {
        var lexer = CreateLexer(input);
        foreach (var expected in expectedTokens)
        {
            var token = lexer.NextToken();
            Assert.Equal(expected, token.TokenType);
        }

        Assert.Equal(TokenType.EOF, lexer.NextToken().TokenType);
    }

    [Theory]
    [InlineData("123", TokenType.Number, 123d)]
    [InlineData("Foo", TokenType.Identifier, "Foo")]
    [InlineData("TheBar\n", TokenType.Identifier, "TheBar")]
    public void NextToken_TokenWithValue_ReturnsExpectedTypeAndValue(string input, TokenType type, object value)
    {
        var token = CreateLexer(input).NextToken();
        Assert.Equal(type, token.TokenType);
        Assert.NotNull(token.Value);
        Assert.Equal(value, token.Value);
    }

    [Theory]
    [InlineData("\"hello\"", "hello")]
    [InlineData("\"hello world\"", "hello world")]
    [InlineData("\"Hello\\nworld\"", "Hello\nworld")]
    public void NextToken_String_ReturnsStringValue(string input, string expected)
    {
        var token = CreateLexer(input).NextToken();

        Assert.Equal(TokenType.String, token.TokenType);
        Assert.Equal(expected, token.Value);
    }

    [Theory]
    [InlineData("123 %", 4, 1)]
    [InlineData("@a = Foo(\"bar\")\\;", 15, 7)]
    [InlineData("$", 0, 0)]
    public void NextToken_UnexpectedCharacter_ThrowsUnexpectedCharacter(string input, int position, int tokenNumber)
    {
        var cliCodeStream = new CliCodeStream(input);
        var lexer = new Lexer(cliCodeStream);
        for (int i = 0; i < tokenNumber; i++)
            lexer.NextToken();
        
        Assert.Throws<Exception>(lexer.NextToken);
        Assert.Equal(position, cliCodeStream.Position);
    }
    
    [Fact]
    public void Peek_DoesNotAdvanceLexer()
    {
        var stream = new CliCodeStream("123   + 5");
        var lexer = new Lexer(stream);

        int oldPosition = stream.Position;

        var peeked = lexer.Peek();

        int positionAfter = stream.Position;

        Assert.Equal(oldPosition, positionAfter);

        var actual = lexer.NextToken();

        Assert.Equal(peeked, actual);
        
        lexer.Peek();
    }
}
