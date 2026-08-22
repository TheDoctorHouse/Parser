using TheParser.Lexing;
using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public class OperationInterpretationException : RuntimeException
{
    public OperationInterpretationException(Interpretation left, TokenType @operator, Interpretation right, SourceSpan span) :
     base($"Cannot solve binary operation `{left.GetType().Name} {@operator} {right.GetType().Name}", span)
    { }

    public OperationInterpretationException(Interpretation left, TokenType @operator, SourceSpan span) :
     base($"Cannot solve unary operation `{@operator} {left.GetType().Name}", span)
    { }
}
