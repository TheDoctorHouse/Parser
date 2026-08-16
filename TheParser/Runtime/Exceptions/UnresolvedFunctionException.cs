namespace TheParser.Runtime.Exceptions;

public class UnresolvedFunctionException(string identifier) :
    RuntimeException($"Cannot resolve function call `{identifier}`");