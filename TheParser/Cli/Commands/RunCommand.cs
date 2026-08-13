using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Runtime;
using TheParser.Syntax;
using TheParser.Debugging;
using TheParser.Cli.Attributes;
using TheParser.Contracts;

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
            var token = lexer.NextToken();
            Console.WriteLine("Lexer:");
            while (token.TokenType != TokenType.EOF)
            {
                if (token.Value != null)
                    Console.Write($"{token.TokenType}({token.Value}) ");
                else
                    Console.Write($"{token.TokenType} ");
                token = lexer.NextToken();
            }

            Console.Write(token.TokenType);
        }

        lexer.Reset();

        Parser parser = new(lexer, cliCodeStream);

        Statement statement;

        statement = parser.ParseBlockStatement();

        if (debug)
        {
            Console.WriteLine("\nAst builder: ");

            var printer = new AstPrinter();

            string tree = printer.Print(statement);
            Console.WriteLine(tree);
            Console.WriteLine("Interpreter: ");
        }

        Interpreter interpreter = new();

        interpreter.InterpretStatement(statement);

        return CliCommandResult.Success();
    }
}