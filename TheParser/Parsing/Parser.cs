using TheParser.Syntax;
using TheParser.Lexing;
using TheParser.Contracts;

namespace TheParser.Parsing;

public class Parser(Lexer lexer, ICodeStream codeStream)
{
    public Statement Parse()
    {
        AstBuilder builder = new AstBuilder(lexer, codeStream);

        return builder.ParseBlockStatement();
    }
}