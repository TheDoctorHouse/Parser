using System.Text;

public static class DebugUtility
{
    private const int SHOW_LETTERS = 15;

    public static string PingPosition(int position, string content) 
    {
        var sb = new StringBuilder();

        int start = position - SHOW_LETTERS;
        int end = position + SHOW_LETTERS;

        bool sliceEnd = end < content.Length;
        bool sliceStart = start < 0;

        int arrowCol;

        if (sliceStart)
        {
            arrowCol = position;
            start = 0;
        }
        else
        {
            sb.Append("...");
            arrowCol = 3 + (position - start);
        }


        if (!sliceEnd)
            end = content.Length - 1;

        sb.Append(content[start..(end+1)]);
        
        if (sliceEnd)
            sb.Append("...");

        AppendWhitespaces(sb, arrowCol);
        sb.Append('↑');
        AppendWhitespaces(sb, arrowCol);
        sb.Append('|');
        AppendWhitespaces(sb, arrowCol);
        sb.Append('|');
        AppendWhitespaces(sb, arrowCol - 2);
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