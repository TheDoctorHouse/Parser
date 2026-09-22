using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class SpaceDefinitionException(string message, SourceSpan span) : RuntimeException(message, span);