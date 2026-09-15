using TheParser.Cli.Attributes;
using TheParser.Debugging;
using TheParser.Debugging.Exceptions;
using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Syntax;

namespace TheParser.Cli.Commands;

[CommandName("ast-code")]
[CommandResource(Resource.AstCodeDescription)]
[RequirePositionals("code")]
public class AstCodeCommand : CliCommand
{
    public override CliCommandResult Run(ArgumentsProvider argumentsProvider)
    {
        string content = argumentsProvider.ReadPositioned(0);

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