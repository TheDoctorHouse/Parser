using System.Reflection;
using TheParser.Cli.Attributes;

namespace TheParser.Cli.Commands;

public class CommandType(Type type, string name, Resource description, string[]? supportedFlags, string[]? positionalNames, string[]? optionalPositionalNames)
{
    public Type Type { get; } = type;
    public string Name { get; } = name;
    public string[]? SupportedFlags { get; } = supportedFlags;
    public string[]? PositionalNames { get; } = positionalNames;
    public Resource DescriptionResource { get; } = description;
    public string[]? OptionalPositionalNames { get; } = optionalPositionalNames;
}

public static class CommandHelper
{
    private readonly static Dictionary<string, CommandType> s_commandTypes = [];

    static CommandHelper()
    {
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass
                    && !t.IsAbstract
                    && typeof(CliCommand).IsAssignableFrom(t));

        foreach (var t in types)
        {
            // Assuming every command has CommandNameAttribute and ResourceNameAttribute.
            // todo: add unit test validating that every command has CommandNameAttribute and ResourceNameAttribute.

            string commandName = GetAttributeOrFail<CommandNameAttribute>(t).CommandName;
            Resource resourceName = GetAttributeOrFail<CommandResourceAttribute>(t).Resource;
            string[]? supportedFlags = t.GetCustomAttribute<SupportedFlagsAttribute>()?.Flags;
            string[]? positionalNames = t.GetCustomAttribute<RequirePositionalsAttribute>()?.Names;
            string[]? optionalPositionalNames = t.GetCustomAttribute<OptionalPositionalsAttribute>()?.Names;

            CommandType commandType = new(t, commandName, resourceName, supportedFlags, positionalNames, optionalPositionalNames);

            s_commandTypes.Add(commandName, commandType);
        }
    }

    private static T GetAttributeOrFail<T>(Type t) where T : Attribute
    {
        return t.GetCustomAttribute<T>() ??
            throw new InvalidOperationException($"Command type '{t.FullName}' is missing {typeof(T).Name}");
    }

    /// <summary>
    /// Gets the command by name.
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <returns>The command type if command exists; otherwise null.</returns>
    public static CommandType? GetCommandType(string commandName)
    {
        if (!s_commandTypes.TryGetValue(commandName, out CommandType? commandType))
            return null;

        return commandType;
    }
}