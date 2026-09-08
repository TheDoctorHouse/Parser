using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class UnexpectedTypeException(string message, SourceSpan span) : RuntimeException(message, span);