using System.Text;
using TheParser.Cli.Attributes;
using TheParser.Debugging;
using TheParser.Debugging.Exceptions;
using TheParser.Lexing;

namespace TheParser.Cli.Commands;

[CommandName("lex-code")]
[RequirePositionals("code")]
[CommandResource(Resource.LexCodeDescription)]
public class LexCodeCommand : CliCommand
{
    public override CliCommandResult Run(ArgumentsProvider argumentsProvider)
    {
        string code = argumentsProvider.ReadPositioned(0);

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