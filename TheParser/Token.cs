using System.Text;

namespace TheParser;

public record Token(TokenType TokenType, object? Value, int Position)
{
    public void AppendDebugInfo(Lexer lexer, StringBuilder sb)
    {
        sb.AppendLine($"TokenType: {TokenType}");
        if (Value == null)
            sb.AppendLine("No value");
        else
            sb.AppendLine($"Value: {Value}");

        sb.AppendLine(DebugUtility.PingPosition(Position, lexer.Content));
    }

    public string GetDebugInfo(Lexer lexer)
    {
        StringBuilder sb = new StringBuilder();
        AppendDebugInfo(lexer, sb);
        return sb.ToString();
    }
}