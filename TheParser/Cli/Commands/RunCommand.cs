using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Runtime;
using TheParser.Syntax;
using TheParser.Debugging;
using TheParser.Cli.Attributes;
using TheParser.Contracts;
using TheParser.Debugging.Exceptions;

namespace TheParser.Cli.Commands;

[CommandName("run")]
[CommandResource(Resource.RunDescription)]
[RequirePositionals("path")]
[SupportedFlags("--debug")]
public sealed class RunCommand : CliCommand
{
    public override Resource CommandDescription => Resource.RunDescription;

    public override CliCommandResult Run(ArgumentsProvider provider)
    {
        string filePath = provider.ReadPositioned(0);

        if (!File.Exists(filePath))
        {
            return IncorrectUsage($"File '{filePath}' does not exist.");
        }

        bool debug = provider.HasArgument("--debug");

        string content = File.ReadAllText(filePath);

        ICodeStream cliCodeStream = new CliCodeStream(content);
        Lexer lexer = new(cliCodeStream);

        if (debug)
        {
            Token token;
            try
            {
                token = lexer.NextToken();
            }
            catch (LanguageException ex)
            {
                return CliCommandResult.Fail(ex, BuildFailMessage(ex, cliCodeStream));
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
                    return CliCommandResult.Fail(ex, BuildFailMessage(ex, cliCodeStream));
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
            return CliCommandResult.Fail(ex, BuildFailMessage(ex, cliCodeStream));
        }

        if (debug)
        {
            Console.WriteLine("\nAst builder: ");

            var printer = new AstPrinter();

            string tree = printer.Print(statement);
            Console.WriteLine(tree);
            Console.WriteLine("Interpreter: ");
        }

        Interpreter interpreter = new();

        try
        {
            interpreter.InterpretStatement(statement);
        }
        catch (LanguageException ex)
        {
            return CliCommandResult.Fail(ex, BuildFailMessage(ex, cliCodeStream));
        }

        return CliCommandResult.Success();
    }

    private static string BuildFailMessage(LanguageException ex, ICodeStream codeStream)
    {
        string message = DebugUtility.PingPosition(codeStream, ex.Span);
        var pos = ex.Span.Start;
        message += $"\nLine {codeStream.GetLineNumber(pos) + 1}, position {pos - codeStream.GetLineStart(pos) + 1}.";
        return message;
    }
}