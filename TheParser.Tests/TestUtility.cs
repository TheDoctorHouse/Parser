using System.Security.Cryptography;
using TheParser.Cli;
using TheParser.Cli.IO;
using TheParser.Runtime.IO;
using TheParser.Tests;

public static class TestUtility
{
    public static DependencyInjector CreateConsoleDependencyInjector()
    {
        var depInjector = new DependencyInjector();
        depInjector.AddSingleton<IPrinter>(new ConsolePrinter());
        depInjector.AddSingleton<IReader>(new ConsoleReader());
        return depInjector;
    }
}