using TheParser.Cli.Attributes;
using TheParser.Debugging;
using TheParser.Debugging.Exceptions;
using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Syntax;

namespace TheParser.Cli.Commands;

[CommandName("ast")]
[CommandResource(Resource.AstCodeDescription)]
[RequirePositionals("path")]
public class AstCommand : CliCommand
{
    public override CliCommandResult Run(ArgumentsProvider argumentsProvider)
    {
        string filePath = argumentsProvider.ReadPositioned(0);

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

        var printer = new AstPrinter();
        Console.WriteLine(printer.Print(statement));
        return CliCommandResult.Success();
    }
}