using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Runtime;
using TheParser.Syntax;
using TheParser.Debugging;
using TheParser.Cli.Attributes;
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
                return CliCommandResult.Fail(ex, BuildFailMessage(ex, content));
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
                    return CliCommandResult.Fail(ex, BuildFailMessage(ex, content));
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
            return CliCommandResult.Fail(ex, BuildFailMessage(ex, content));
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
            return CliCommandResult.Fail(ex, BuildFailMessage(ex, content));
        }

        return CliCommandResult.Success();
    }

    private static string BuildFailMessage(LanguageException ex, string content)
    {
        string message = DebugUtility.PingPosition(content, ex.Span);
        var pos = ex.Span.Start;
        int line = GetLineNumber(content, pos);
        int linePos = pos - GetLineStart(content, pos);
        message += $"\nLine {line + 1}, position {linePos + 1}.";
        return message;
    }

    private static int GetLineNumber(string content, int position)
    {
        int lineNumber = 0;
        int currentPosition = 0;

        while (currentPosition != position)
        {
            if (content[currentPosition] == '\n')
                lineNumber++;
            currentPosition++;
        }

        return lineNumber;
    }

    public static int GetLineStart(string content, int position)
    {
        if (content[position] == '\n')
            position--;

        if (position == 0)
            return position;
        var start = content.LastIndexOf('\n', position);
        return start == -1 ? 0 : start + 1;
    }
}