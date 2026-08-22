using TheParser.Debugging.Exceptions;
using TheParser.Syntax;

namespace TheParser.Parsing.Exceptions;

public abstract class ParserException(string message, SourceSpan span) : LanguageException(message, span);