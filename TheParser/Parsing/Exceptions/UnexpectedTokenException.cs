using TheParser.Lexing;
using TheParser.Syntax;

namespace TheParser.Parsing.Exceptions;

public class UnexpectedTokenException(
    TokenType received,
    SourceSpan span,
    params TokenType[] expected
) : ParserException("Got an unexpected token during parsing.", span)
{
    public TokenType[] ExpectedTokens { get; } = expected;
    public TokenType ReceivedToken { get; } = received;
}