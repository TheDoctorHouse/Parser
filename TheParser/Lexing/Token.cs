using System.Text;
using TheParser.Contracts;
using TheParser.Debugging;

namespace TheParser.Lexing;

public record Token(TokenType TokenType, object? Value, int Position)
{
    public void AppendDebugInfo(ICodeStream codeStream, StringBuilder sb)
    {
        sb.AppendLine($"TokenType: {TokenType}");
        if (Value == null)
            sb.AppendLine("No value");
        else
            sb.AppendLine($"Value: {Value}");

        sb.AppendLine(DebugUtility.PingPosition(Position, codeStream));
    }

    public string GetDebugInfo(ICodeStream codeStream)
    {
        StringBuilder sb = new ();
        AppendDebugInfo(codeStream, sb);
        return sb.ToString();
    }
}