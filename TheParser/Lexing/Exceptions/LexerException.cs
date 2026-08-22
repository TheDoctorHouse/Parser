using TheParser.Debugging.Exceptions;
using TheParser.Syntax;

namespace TheParser.Lexing.Exceptions;

public abstract class LexerException(string message, SourceSpan span) : LanguageException(message, span);