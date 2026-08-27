using TheParser.Cli.IO;
using TheParser.DependencyInjection;
using TheParser.Runtime.Functions.BuiltIns;
using TheParser.Runtime.IO;

namespace TheParser.Tests;

public class DependencyInjectorTests
{
    [Theory]
    [InlineData(typeof(Print))]
    [InlineData(typeof(Ask))]
    public void CreateInstance_MissingDependency_ThrowsInvalidOperationException(Type type)
    {
        var injector = new DependencyInjector();
        Assert.Throws<InvalidOperationException>(() => injector.CreateInstance(type));
    }

    [Theory]
    [InlineData(typeof(Print))]
    [InlineData(typeof(Ask))]
    public void CommandType_ProvidedDependency_DoesNotThrow(Type type)
    {
        var injector = new DependencyInjector();
        injector.AddSingleton<IPrinter>(new ConsolePrinter());
        injector.AddSingleton<IReader>(new ConsoleReader());
        Assert.NotNull(injector.CreateInstance(type));
    }
}