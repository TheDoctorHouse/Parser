using System.Text;
using TheParser.Cli.Attributes;
using TheParser.Debugging;
using TheParser.Debugging.Exceptions;
using TheParser.Lexing;

namespace TheParser.Cli.Commands;

[CommandName("lex")]
[RequirePositionals("path")]
[CommandResource(Resource.LexDescription)]
public class LexCommand : CliCommand
{
    public override CliCommandResult Run(ArgumentsProvider argumentsProvider)
    {
        string filePath = argumentsProvider.ReadPositioned(0);

        if (!File.Exists(filePath))
        {
            return IncorrectUsage($"File '{filePath}' does not exist.");
        }

        string code = File.ReadAllText(filePath);

        StringBuilder tokens = new();

        var lexer = new Lexer(code);

        Token token;
        try
        {
            token = lexer.NextToken();
        }
        catch (LanguageException ex)
        {
            return CliCommandResult.Fail(ex, DebugUtility.BuildFailMessage(ex, code));
        }

        while (token.TokenType != TokenType.EOF)
        {
            if (token.Value != null)
                tokens.Append($"{token.TokenType}({token.Value}) ");
            else
                tokens.Append($"{token.TokenType} ");
            try
            {
                token = lexer.NextToken();
            }
            catch (LanguageException ex)
            {
                return CliCommandResult.Fail(ex, DebugUtility.BuildFailMessage(ex, code));
            }
        }

        Console.WriteLine(tokens.ToString());

        return CliCommandResult.Success();
    }
}
