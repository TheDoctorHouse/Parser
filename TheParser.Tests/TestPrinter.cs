using TheParser.Runtime.IO;

namespace TheParser.Tests;

public class TestPrinter : IPrinter
{
    private readonly Queue<string> _prints = new();

    public void Print(string content)
    {
        _prints.Enqueue(content);
    }

    public bool TryDequeue(out string? content)
    {
        return _prints.TryDequeue(out content);
    }
}