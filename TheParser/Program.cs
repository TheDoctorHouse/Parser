using TheParser.Cli;
using TheParser.Cli.Commands;
using TheParser.Cli.IO;
using TheParser.DependencyInjection;
using TheParser.Runtime.IO;

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

var injector = new DependencyInjector();
injector.AddSingleton<IReader>(new ConsoleReader());
injector.AddSingleton<IPrinter>(new ConsolePrinter());

var runner = new CommandRunner(commandType, injector);

try
{
    return runner.Run(args);
}
catch (Exception ex)
{
    Console.Error.WriteLine("Program: Failed with an internal exception.");
    Console.Error.WriteLine(ex);

    return CliCommandResult.FAIL;
}