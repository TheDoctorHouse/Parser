using TheParser.Cli.IO;
using TheParser.DependencyInjection;
using TheParser.Runtime.IO;

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