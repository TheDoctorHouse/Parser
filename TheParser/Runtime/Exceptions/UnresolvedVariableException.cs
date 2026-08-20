namespace TheParser.Runtime.Exceptions;

public class UnresolvedVariableException(string identifier) : RuntimeException($"No such declaration `{identifier}`");
