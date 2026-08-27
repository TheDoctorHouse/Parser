using System.Reflection;
using TheParser.Cli.Attributes;
using TheParser.Cli.Commands;

namespace TheParser.Tests;

public class CliTests
{
    private static IEnumerable<Type> GetCommandTypes()
    {
        var assembly = typeof(CliCommand).Assembly;

        var commands = assembly.GetTypes().Where(
            t => !t.IsAbstract &&
            typeof(CliCommand).IsAssignableFrom(t));

        return commands;
    }

    [Fact]
    public void CliCommandTypes_HasExactlyOneConstructor()
    {
        foreach (var commandType in GetCommandTypes())
        {
            var constructors = commandType.GetConstructors();
            Assert.Single(constructors);
        }
    }

    [Fact]
    public void CliCommandTypes_HasRequiredAttributes()
    {
        foreach (var commandType in GetCommandTypes())
        {
            Assert.NotNull(commandType.GetCustomAttribute<CommandNameAttribute>());
            Assert.NotNull(commandType.GetCustomAttribute<CommandResourceAttribute>());
        }
    }
}