using System.Diagnostics;
using TheParser.Contracts;

namespace TheParser.Cli;

public class CliCodeStream(string content) : ICodeStream
{
    public int Position { get; private set; } = 0;

    public char? Current { get; private set; } = content.Length > 0 ? content[0] : null;

    public int Length => content.Length;
    public char? Next()
    {
        Position++;
        if (Position >= content.Length)
        {
            Current = null;
            return null;
        }

        Current = content[Position];
        return Current;
    }

    public char? Peek()
    {
        return Position <= content.Length - 2 ? content[Position + 1] : null;
    }

    public void Seek(int position)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(position, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(position, Length);

        Position = position;
        
        if (position >= content.Length)
        {
            Current = default;
            return;
        }

        Current = content[position];
        return;
    }
    public string GetLine(int position)
    {
        int start = GetLineStart(position);
        int end = GetLineEnd(position);

        Debug.Assert(start >= 0);
        Debug.Assert(end <= content.Length);
        return content[start..end];
    }

    public int GetLineEnd(int position)
    {
        int end = content.IndexOf('\n', position);
        return end == -1 ? content.Length : end;
    }

    public int GetLineNumber(int position)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position, content.Length);

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

    public int GetLineStart(int position)
    {
        var start = content.LastIndexOf('\n', position);
        return start == -1 ? 0 : start + 1;
    }
}