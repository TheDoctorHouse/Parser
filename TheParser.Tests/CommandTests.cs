using System.Diagnostics;
using TheParser.Cli;
using TheParser.Cli.Commands;
using TheParser.Debugging.Exceptions;

namespace TheParser.Tests;

public class CommandTests
{
    private static string RemoveNonAlphanumeric(string input)
    {
        return string.Concat(input.Where(c => char.IsLetter(c) || char.IsDigit(c)));
    }

    [Theory]
    [InlineData("Dollar", 1, 1)]
    [InlineData("NotANumberConversion", 3, 25)]
    public void RunCommand_IncorrectOutput_ReportsCorrectSourceLocation(string scriptName, int line, int position)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Scripts", scriptName);
        Assert.True(File.Exists(path), $"Script {path} was not found.");
        var command = new RunCommand();
        var argumentsProvider = new ArgumentsProvider(["run", path]);
        var commandType = CommandHelper.GetCommandType("run");
        Assert.NotNull(commandType);
        Assert.Null(argumentsProvider.Validate(commandType));
        CliCommandResult res = command.Run(argumentsProvider);
        Assert.Equal(CliCommandResult.FAIL, res.ResultCode);

        Assert.NotNull(res.Description);
        Assert.IsType<LanguageException>(res.Exception, exactMatch: false);

        string normalizedDescription = RemoveNonAlphanumeric(res.Description);

        Assert.Contains("position" + position, normalizedDescription, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("line" + line, normalizedDescription, StringComparison.OrdinalIgnoreCase);

    }
}