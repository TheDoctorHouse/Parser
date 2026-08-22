using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class FunctionInvocationException(string message, SourceSpan span, FunctionException inner)
    : RuntimeException(message + $"\nInner Exception `{inner.GetType().Name}`:\n{inner.Message}", span)
{
    public FunctionException Inner => inner;
}