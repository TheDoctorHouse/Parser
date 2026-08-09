namespace TheParser.Debugging;

using System.Text;

public static class DebugUtility
{
    private const int SHOW_LETTERS = 15;

    public static string PingPosition(int position, string content) 
    {
        var sb = new StringBuilder();

        int lineStart = content.LastIndexOf('\n', Math.Max(0, position - 1));
        lineStart = lineStart == -1 ? 0 : lineStart + 1;

        int lineEnd = content.IndexOf('\n', position);
        lineEnd = lineEnd == -1 ? content.Length : lineEnd;

        if (lineEnd > lineStart && content[lineEnd - 1] == '\r')
            lineEnd--;

        int start = Math.Max(lineStart, position - SHOW_LETTERS);
        int end = Math.Min(lineEnd, position + SHOW_LETTERS + 1);

        bool sliceStart = start > lineStart;
        bool sliceEnd = end < lineEnd;

        int arrowCol = position - start;

        if (sliceStart)
        {
            sb.Append("...");
            arrowCol += 3;
        }

        sb.Append(content[start..end]);

        if (sliceEnd)
            sb.Append("...");

        AppendWhitespaces(sb, arrowCol);
        sb.Append('^');
        AppendWhitespaces(sb, arrowCol);
        sb.Append('|');
        AppendWhitespaces(sb, arrowCol);
        sb.Append('|');
        AppendWhitespaces(sb, Math.Max(0, arrowCol - 2));
        sb.Append("HERE");

        return sb.ToString();
    }

    private static void AppendWhitespaces(StringBuilder sb, int count)
    {
        sb.AppendLine();
        for (int i = 0; i < count; i++)
            sb.Append(' ');
    }
}