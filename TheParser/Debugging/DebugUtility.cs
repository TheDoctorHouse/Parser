using System.Text;
using TheParser.Debugging.Exceptions;
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

    public static string BuildFailMessage(LanguageException ex, string content)
    {
        string message = PingPosition(content, ex.Span);
        var pos = ex.Span.Start;
        int line = GetLineNumber(content, pos);
        int linePos = pos - GetLineStart(content, pos);
        message += $"\nLine {line + 1}, position {linePos + 1}.";
        return message;
    }

    public static int GetLineNumber(string content, int position)
    {
        int lineNumber = 0;
        int currentPosition = 0;

        while (currentPosition != position)
        {
            if (content[currentPosition] == '\n')
                lineNumber++;
            currentPosition++;
        }

        return lineNumber;
    }

    public static int GetLineStart(string content, int position)
    {
        if (content[position] == '\n')
            position--;

        if (position == 0)
            return position;
        var start = content.LastIndexOf('\n', position);
        return start == -1 ? 0 : start + 1;
    }
}