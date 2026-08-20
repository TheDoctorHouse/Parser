using TheParser.Lexing;

namespace TheParser.Runtime.Exceptions;

public class OperationInterpretationException : RuntimeException
{
    public OperationInterpretationException(Interpretation left, TokenType @operator, Interpretation right) :
     base($"Cannot solve binary operation `{left.GetType().Name} {@operator} {right.GetType().Name}")
    { }

    public OperationInterpretationException(Interpretation left, TokenType @operator) :
     base($"Cannot solve unary operation `{@operator} {left.GetType().Name}")
    { }
}
