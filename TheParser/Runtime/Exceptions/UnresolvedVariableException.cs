using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class UnresolvedVariableException(string identifier, SourceSpan span)
    : RuntimeException($"No such declaration `{identifier}`", span);
