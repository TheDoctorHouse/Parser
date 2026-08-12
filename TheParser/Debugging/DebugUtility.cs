namespace TheParser.Debugging;

using System.Text;
using TheParser.Contracts;

public static class DebugUtility
{
    private const int SHOW_LETTERS = 15;

    public static string PingPosition(int position, ICodeStream codeStream) 
    {
        var sb = new StringBuilder();

        string line = codeStream.GetLine(position);
        int lineStart = codeStream.GetLineStart(position);
        int relativePosition = position - lineStart;

        int start = Math.Max(0, relativePosition - SHOW_LETTERS);
        int end = Math.Min(line.Length, relativePosition + SHOW_LETTERS + 1);
        Console.WriteLine(line);
        Console.WriteLine($"Start {start} end {end}");

        int arrowCol = position - lineStart;

        bool sliceStart = start > 0;
        bool sliceEnd = end < line.Length - 1;

        if (sliceStart)
        {
            sb.Append("...");
            arrowCol += 3;
        }

        sb.Append(line[start..end]);

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