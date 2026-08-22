using TheParser.Cli;
using TheParser.Cli.Commands;
using TheParser.Debugging;
using TheParser.Debugging.Exceptions;

if (args.Length == 0)
{
    var text = CliHelper.Read(Resource.HelpGeneral);
    Console.WriteLine(text);
    return 3;
}


var commandName = args[0];


CommandType? commandType = CommandHelper.GetCommandType(commandName);

if (commandType == null)
{
    var text = CliHelper.Read(Resource.UnknownCommand, commandName);
    Console.Error.WriteLine(text);
    return 3;
}

// Assuming every command has zero arguments.
// todo: unit test validating that command has 0 arguments.
var command = (CliCommand)Activator.CreateInstance(commandType.Type)!;

try
{
    var argumentsProvider = new ArgumentsProvider(args);

    CliCommandResult? result = argumentsProvider.Validate(commandType);

    if (result != null)
        return result.Value.AcknowledgeUser();

    return command.Run(argumentsProvider).AcknowledgeUser();
}
catch (Exception ex)
{
    Console.Error.WriteLine("Program: Failed with an internal exception.");
    Console.Error.WriteLine(ex);

    return CliCommandResult.FAIL;
}