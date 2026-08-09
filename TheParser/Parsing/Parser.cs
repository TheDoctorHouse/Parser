using TheParser.Syntax;
using TheParser.Lexing;

namespace TheParser.Parsing;

public class Parser
{
    private readonly Lexer _tokenizer;

    public Parser(Lexer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public Statement Parse()
    {
        AstBuilder builder = new AstBuilder(_tokenizer);

        return builder.ParseBlockStatement();
    }
}