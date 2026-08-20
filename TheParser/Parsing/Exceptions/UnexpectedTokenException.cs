using TheParser.Lexing;

namespace TheParser.Parsing.Exceptions;

public class UnexpectedTokenException(
    TokenType received,
    params TokenType[] expected
) : ParserException("Got an unexpected token during parsing.")
{
    public TokenType[] ExpectedTokens { get; } = expected;
    public TokenType ReceivedToken { get; } = received;
}