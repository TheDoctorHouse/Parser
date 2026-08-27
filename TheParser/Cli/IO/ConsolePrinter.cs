using TheParser.Runtime.IO;

namespace TheParser.Cli.IO;

public class ConsolePrinter : IPrinter
{
    public void Print(string content)
    {
        Console.Write(content);
    }
}