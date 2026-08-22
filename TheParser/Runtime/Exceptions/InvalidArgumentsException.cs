using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class InvalidArgumentsException(string message, SourceSpan span) : RuntimeException(message, span);