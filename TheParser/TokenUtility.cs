namespace TheParser;

public static class TokenUtility 
{
    public static string OperatorToString(TokenType tokenType)
    {
        return tokenType switch
        {
            TokenType.Plus => "+",
            TokenType.Minus => "-",
            TokenType.Multiply => "*",
            TokenType.Divide => "/",
            _ => throw new InvalidOperationException("Not an operator token: " + tokenType),
        };
    }

    public static bool IsOperator(TokenType tokenType)
    {
        return tokenType is TokenType.Plus or TokenType.Minus or TokenType.Multiply or TokenType.Divide;
    }
}