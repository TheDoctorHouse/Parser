using TheParser.Syntax;

namespace TheParser.Lexing.Exceptions;

public class UnexpectedCharacterException(string message, SourceSpan span) : LexerException(message, span);