using TheParser.Runtime.IO;

namespace TheParser.Cli.IO;

public class ConsoleReader : IReader
{
    public string? ReadLine()
    {
        return Console.ReadLine();
    }
}