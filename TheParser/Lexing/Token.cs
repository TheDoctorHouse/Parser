using System.Text;
using TheParser.Contracts;
using TheParser.Debugging;

namespace TheParser.Lexing;

public record Token(TokenType TokenType, object? Value, int Position);