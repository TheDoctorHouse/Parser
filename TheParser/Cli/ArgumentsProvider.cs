using System.Text;
using TheParser.Cli.Commands;

namespace TheParser.Cli;

public class ArgumentsProvider(string[] arguments)
{
    public string ReadPositioned(int position)
    {
        return arguments[position + 1];// ignoring first argument as it is a command name.
    }

    public string? ReadOptionalPositioned(int position)
    {
        if (position + 1 >= arguments.Length)
            return null;

        if (arguments[position + 1].StartsWith('-'))
            return null;

        return arguments[position + 1];
    }

    public bool HasArgument(string argumentName)
    {
        return arguments.Contains(argumentName);
    }

    /// <summary>
    /// Validate the arguments.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>null if command was validated; otherwise incorrect usage result.</returns>
    public CliCommandResult? Validate(CommandType command)
    {
        int positionalNamesLength = command.PositionalNames?.Length ?? 0;
        if (arguments.Length - 1 < positionalNamesLength) // -1 here to ignore command-name argument
        {
            var message = $"Missing one or more positional arguments.\nUsage: {CliHelper.BuildUsage(command)}";
            return CliCommandResult.IncorrectUsage(message);
        }

        StringBuilder unexpectedArguments = new("Unexpected argument(s): ");

        bool wasUnexpectedArgument = false;
        bool first = true;

        for (int i = positionalNamesLength + 1; i < arguments.Length; i++)
        {
            int optionalPositionalsCount = command.OptionalPositionalNames?.Length ?? 0;
            bool optionalPositionalArea = i < positionalNamesLength + 1 + optionalPositionalsCount;
            bool supportedFlagContains = command.SupportedFlags != null && command.SupportedFlags.Contains(arguments[i]);
            bool isOptionalPositional = optionalPositionalArea && !arguments[i].StartsWith('-');
            if (isOptionalPositional || supportedFlagContains || CliHelper.IsGeneralArgument(arguments[i]))
                continue;


            wasUnexpectedArgument = true;
            if (!first)
                unexpectedArguments.Append(", ");
            else
                first = false;
            unexpectedArguments.Append(arguments[i]);

        }

        unexpectedArguments.Append('.');

        if (wasUnexpectedArgument)
            return CliCommandResult.IncorrectUsage(unexpectedArguments.ToString());

        return null;
    }
}