using System.Diagnostics;
using System.Reflection;
using TheParser.Cli;
using TheParser.Cli.Attributes;
using TheParser.Cli.Commands;
using TheParser.Debugging.Exceptions;
using TheParser.DependencyInjection;
using TheParser.Runtime.IO;

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

        var command = new RunCommand(TestUtility.CreateConsoleDependencyInjector());

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

    [Theory]
    [InlineData("Boolean", "True")]
    public void RunCommand_CorrectInput_OutputsCorrectContent(string scriptName, string expectedOutput)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Scripts", scriptName);
        Assert.True(File.Exists(path), $"Script {path} was not found.");

        var injector = new DependencyInjector();
        var printer = new TestPrinter();
        injector.AddSingleton<IReader>(new TestReader());
        injector.AddSingleton<IPrinter>(printer);

        var command = new RunCommand(injector);

        var argumentsProvider = new ArgumentsProvider(["run", path]);
        var commandType = CommandHelper.GetCommandType("run");
        Assert.NotNull(commandType);
        Assert.Null(argumentsProvider.Validate(commandType));
        CliCommandResult res = command.Run(argumentsProvider);
        Assert.Equal(CliCommandResult.SUCCESS, res.ResultCode);

        Assert.True(printer.TryDequeue(out string? str));
        Assert.Equal(expectedOutput, str);
        Assert.False(printer.TryDequeue(out _));
    }

    [Fact]
    public void RunCommand_TestPrinterAndReaderWorks()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Scripts", "ReadAndPrint");
        Assert.True(File.Exists(path), $"Script {path} was not found.");

        var injector = new DependencyInjector();
        var testPrinter = new TestPrinter();
        injector.AddSingleton<IPrinter>(testPrinter);

        const string inputText = "Foo";
        var testReader = new TestReader(inputText);
        injector.AddSingleton<IReader>(testReader);

        var command = new RunCommand(injector);
        var argumentsProvider = new ArgumentsProvider(["run", path]);
        CliCommandResult res = command.Run(argumentsProvider);
        Assert.Equal(CliCommandResult.SUCCESS, res.ResultCode);
        Assert.True(testPrinter.TryDequeue(out var content));
        Assert.Equal(inputText, content);
        Assert.True(testPrinter.TryDequeue(out content));
        Assert.Equal($"The `{inputText}` bar is my favorite.", content);
        Assert.False(testPrinter.TryDequeue(out _));
    }

    [Theory]
    [InlineData("false", "Boolean(False)")]
    [InlineData("true", "Boolean(True)")]
    [InlineData("foo", "Identifier(foo)")]
    public void LexCode_CorrectUsage_ProvidesCorrectOutput(string code, string expected)
    {
        const string commandName = "lex-code";
        var command = new LexCodeCommand();
        var argProvider = new ArgumentsProvider([commandName, code]);
        var commandType = CommandHelper.GetCommandType(commandName);
        Assert.NotNull(commandType);
        Assert.Null(argProvider.Validate(commandType));

        var writer = new StringWriter();
        Console.SetOut(writer);

        command.Run(argProvider);
        Assert.Equal(expected, writer.ToString().Trim());
    }

    [Fact]
    public void HelpCommand_DoesNotThrowWithCorrectCommand()
    {
        Console.SetOut(new StringWriter());
        const string helpCommandName = "help";
        var types = TestUtility.GetCliCommandTypes();

        foreach (var t in types)
        {
            var attr = t.GetCustomAttribute<CommandNameAttribute>();
            Assert.True(attr is not null, $"No command name attribute on command {t.FullName}");
            var name = attr.CommandName;
            var command = new HelpCommand();

            var argumentsProvider = new ArgumentsProvider(["help", name]);
            var commandType = CommandHelper.GetCommandType(helpCommandName);
            Assert.NotNull(commandType);
            Assert.Null(argumentsProvider.Validate(commandType));
            var exception = Record.Exception(() => command.Run(argumentsProvider));
            Assert.Null(exception);
        }
    }
}