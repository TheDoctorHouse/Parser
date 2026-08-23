using System.Text;
using TheParser.Syntax;

namespace TheParser.Debugging;

public static class DebugUtility
{
    private const int SHOW_LETTERS = 15;

    public static string PingPosition(string content, SourceSpan span)
    {
        int start = span.Start;
        int end = span.Start + span.Length;

        if (end > content.Length || start < 0)
            throw new ArgumentOutOfRangeException(nameof(span));

        var sb = new StringBuilder();

        int contentStart = Math.Max(0, start - SHOW_LETTERS);
        int contentEnd = Math.Min(content.Length, end + SHOW_LETTERS);

        sb.Append("...");

        bool firstLine = true;
        int i = contentStart;
        while (i < contentEnd)
        {
            int lineStart = i;
            for (; i < contentEnd; i++)
            {
                var current = content[i];

                if (current == '\r')
                    continue;

                if (current == '\n')
                {
                    i++;
                    break;
                }

                sb.Append(current);
            }

            sb.AppendLine();
            if (firstLine)
                sb.Append("   ");

            for (int j = lineStart; j < i; j++)
            {
                bool isWhiteSpace = char.IsWhiteSpace(content[j]);
                if (isWhiteSpace)
                    sb.Append(' ');
                else if (j >= start && j < end)
                    sb.Append('^');
                else
                    sb.Append(' ');
            }

            sb.AppendLine();
            firstLine = false;
        }

        sb.Append("...");
        return sb.ToString();
    }
}