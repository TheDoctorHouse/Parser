using TheParser.Cli.Commands;
using TheParser.Cli.IO;
using TheParser.DependencyInjection;
using TheParser.Runtime.Functions;
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

    public static IEnumerable<Type> GetCliCommandTypes()
    {
        var assembly = typeof(CliCommand).Assembly;

        var functions = assembly.GetTypes().Where(
            t => !t.IsAbstract &&
            typeof(CliCommand).IsAssignableFrom(t));

        return functions;
    }

    public static IEnumerable<Type> GetBuiltInFunctionTypes()
    {
        var assembly = typeof(BuiltInFunction).Assembly;

        var functions = assembly.GetTypes().Where(
            t => !t.IsAbstract &&
            typeof(BuiltInFunction).IsAssignableFrom(t));

        return functions;
    }
}