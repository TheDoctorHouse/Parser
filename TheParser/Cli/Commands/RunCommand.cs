using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Runtime;
using TheParser.Syntax;
using TheParser.Debugging;
using TheParser.Cli.Attributes;
using TheParser.Debugging.Exceptions;
using TheParser.DependencyInjection;

namespace TheParser.Cli.Commands;

[CommandName("run")]
[CommandResource(Resource.RunDescription)]
[RequirePositionals("path")]
public sealed class RunCommand(DependencyInjector injector) : CliCommand
{
    public override CliCommandResult Run(ArgumentsProvider provider)
    {
        string filePath = provider.ReadPositioned(0);

        if (!File.Exists(filePath))
        {
            return IncorrectUsage($"File '{filePath}' does not exist.");
        }

        string content = File.ReadAllText(filePath);

        Lexer lexer = new(content);

        lexer.Reset();

        Parser parser = new(lexer);

        Statement statement;

        try
        {
            statement = parser.ParseBlockStatement();
        }
        catch (LanguageException ex)
        {
            return CliCommandResult.Fail(ex, DebugUtility.BuildFailMessage(ex, content));
        }

        Interpreter interpreter = new(injector);

        try
        {
            interpreter.InterpretStatement(statement);
        }
        catch (LanguageException ex)
        {
            return CliCommandResult.Fail(ex, DebugUtility.BuildFailMessage(ex, content));
        }

        return CliCommandResult.Success();
    }

}