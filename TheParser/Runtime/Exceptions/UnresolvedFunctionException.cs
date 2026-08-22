using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class UnresolvedFunctionException(string identifier, SourceSpan span)
    : RuntimeException($"Cannot resolve function call `{identifier}`", span);