using TheParser.Cli.Commands;

namespace TheParser.Cli;

public class CommandRunner(CommandType commandType)
{
    public int Run(string[] args)
    {
        var command = (CliCommand)Activator.CreateInstance(commandType.Type)!;

        var argumentsProvider = new ArgumentsProvider(args);

        CliCommandResult? result = argumentsProvider.Validate(commandType);

        if (result != null)
            return result.Value.AcknowledgeUser();

        return command.Run(argumentsProvider).AcknowledgeUser();
    }
}