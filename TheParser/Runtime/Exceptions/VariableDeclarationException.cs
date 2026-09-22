using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class VariableDeclarationException(string message, SourceSpan span) : RuntimeException(message, span);