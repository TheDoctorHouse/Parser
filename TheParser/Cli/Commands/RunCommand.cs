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
[SupportedFlags("--debug")]
public sealed class RunCommand(DependencyInjector injector) : CliCommand
{
    public override CliCommandResult Run(ArgumentsProvider provider)
    {
        string filePath = provider.ReadPositioned(0);

        if (!File.Exists(filePath))
        {
            return IncorrectUsage($"File '{filePath}' does not exist.");
        }

        bool debug = provider.HasArgument("--debug");

        string content = File.ReadAllText(filePath);

        Lexer lexer = new(content);

        if (debug)
        {
            Token token;
            try
            {
                token = lexer.NextToken();
            }
            catch (LanguageException ex)
            {
                return CliCommandResult.Fail(ex, DebugUtility.BuildFailMessage(ex, content));
            }
            Console.WriteLine("Lexer:");
            while (token.TokenType != TokenType.EOF)
            {
                if (token.Value != null)
                    Console.Write($"{token.TokenType}({token.Value}) ");
                else
                    Console.Write($"{token.TokenType} ");
                try
                {
                    token = lexer.NextToken();
                }
                catch (LanguageException ex)
                {
                    return CliCommandResult.Fail(ex, DebugUtility.BuildFailMessage(ex, content));
                }
            }

            Console.Write(token.TokenType);
        }

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

        if (debug)
        {
            Console.WriteLine("\nAst builder: ");

            var printer = new AstPrinter();

            string tree = printer.Print(statement);
            Console.WriteLine(tree);
            Console.WriteLine("Interpreter: ");
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