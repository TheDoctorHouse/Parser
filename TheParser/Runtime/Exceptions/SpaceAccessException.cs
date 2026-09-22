using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class SpaceAccessException(string message, SourceSpan span) : RuntimeException(message, span);
