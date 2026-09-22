using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class SpaceUndefinitionException(string message, SourceSpan span) : RuntimeException(message, span);