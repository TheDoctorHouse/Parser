using TheParser.Runtime.IO;

namespace TheParser.Tests;

public class TestReader(params string[] returns) : IReader
{
    private readonly Queue<string> _returns = new(returns);

    public string? ReadLine()
    {
        return _returns.TryDequeue(out var result) ? result : null;
    }
}