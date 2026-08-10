using TheParser.Cli.Attributes;

namespace TheParser.Cli.Commands;

[CommandName("help")]
[CommandResource(Resource.HelpDescription)]
[OptionalPositionals("command")]
public class HelpCommand : CliCommand
{
    public override Resource CommandDescription => Resource.HelpDescription;

    public override CliCommandResult Run(ArgumentsProvider argumentsProvider)
    {
        string? commandName = argumentsProvider.ReadOptionalPositioned(0);
        if (commandName == null)
        {
            var text = CliHelper.Read(Resource.HelpGeneral);
            Console.WriteLine(text);
            return CliCommandResult.Success();
        }

        CommandType? command = CommandHelper.GetCommandType(commandName);

        if (command == null)
        {
            return CliCommandResult.IncorrectUsage(CliHelper.Read(Resource.UnknownCommand, commandName));
        }

        Console.WriteLine(CliHelper.Read(command.DescriptionResource, CliHelper.BuildUsage(command)));

        return CliCommandResult.Success();
    }
}