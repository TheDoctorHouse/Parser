using System.Runtime.InteropServices.Marshalling;
using System.Text;
using TheParser.Contracts;
using TheParser.Syntax;

namespace TheParser.Debugging;

public static class DebugUtility
{
    private const int SHOW_LETTERS = 15;

    public static string PingPosition(ICodeStream codeStream, SourceSpan span)
    {
        int start = span.Start;
        int end = span.Start + span.Length;

        if (end > codeStream.Length || start < 0)
            throw new ArgumentOutOfRangeException(nameof(span));

        var sb = new StringBuilder();

        int contentStart = Math.Max(0, start - SHOW_LETTERS);
        int contentEnd = Math.Min(codeStream.Length, end + SHOW_LETTERS);

        codeStream.Seek(contentStart - 1);

        sb.Append("...");

        bool firstLine = true;
        int i = contentStart;
        while (i < contentEnd)
        {
            int lineStart = i;
            for (; i <= contentEnd; i++)
            {
                var next = codeStream.Next();
                if (next == null)
                    continue;

                if (next.Value == '\r')
                    continue;

                if (next.Value == '\n')
                {
                    i++;
                    break;
                }

                sb.Append(next.Value);
            }

            sb.AppendLine();
            if (firstLine)
                sb.Append("   ");

            for (int j = lineStart; j < i; j++)
            {
                int prevState = codeStream.Position;
                // todo: simplify ICodeStream so that random access becomes easier.
                // temporary hack
                codeStream.Seek(j);
                bool isWhiteSpace = codeStream.Current.HasValue && char.IsWhiteSpace(codeStream.Current.Value);
                codeStream.Seek(prevState);
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