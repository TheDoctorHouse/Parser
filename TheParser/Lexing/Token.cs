using System.Text;
using TheParser.Debugging;

namespace TheParser.Lexing;

public record Token(TokenType TokenType, object? Value, int Position);