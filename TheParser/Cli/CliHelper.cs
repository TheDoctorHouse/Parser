using System.Reflection;
using System.Text;
using TheParser.Cli.Commands;

namespace TheParser.Cli;

public enum Resource
{
    HelpGeneral,
    UnknownCommand,
    InterpretDescription,
    HelpDescription,
}

public static class CliHelper
{
    private const string ResourcePrefix = "TheParser.Cli";

    private static readonly HashSet<string> s_generalArguments = ["--stacktrace"];

    public static bool IsGeneralArgument(string arg) => s_generalArguments.Contains(arg);

    public static string Read(Resource res, params object[] args) 
        => ReadResource(ResourceToString(res), args);

    private static string ReadResource(string name, params object[] args)
    {   
        string resourceName = $"{ResourcePrefix}.{name}.txt";
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName) 
            ?? throw new InvalidOperationException($"Resource '{resourceName}' was not found.");

        using var reader = new StreamReader(stream);
        return string.Format(reader.ReadToEnd(), args);
    }

    public static string BuildFailMessage(string failReason, Resource commandDescription)
    {
        return $"Failed. {failReason}\n{Read(commandDescription)}";
    }

    private static string ResourceToString(Resource resource)
    {
        return resource switch
        {
            Resource.HelpGeneral => "Help.Help",
            Resource.HelpDescription => "Help.Commands.Help",
            Resource.InterpretDescription => "Help.Commands.Interpret",
            Resource.UnknownCommand => "Help.UnknownCommand",
            _ => throw new ArgumentOutOfRangeException(nameof(resource), resource, null),
        };
    }

    public static string BuildUsage(CommandType command)
    {
        var sb = new StringBuilder();

        sb.Append(command.Name);

        if (command.PositionalNames != null)
        {
            foreach (var positional in command.PositionalNames)
                sb.Append($" <{positional}>");
        }

        if (command.OptionalPositionalNames != null)
        {
            foreach (var optional in command.OptionalPositionalNames)
                sb.Append($" [{optional}]");
        }

        if (command.SupportedFlags != null)
        {
            foreach (var optional in command.SupportedFlags)
                sb.Append($" [{optional}]");
        }

        return sb.ToString();
    }
}