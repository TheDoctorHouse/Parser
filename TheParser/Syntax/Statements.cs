using TheParser.Lexing;

namespace TheParser.Syntax;

public abstract record Statement(SourceSpan Span);

public record BlockStatement(IReadOnlyList<Statement> Statements, SourceSpan Span) : Statement(Span);

public record ExpressionStatement(Expr Callee, SourceSpan Span) : Statement(Span);

public record VariableDeclarationStatement(Token Identifier, Expr? Initializer, SourceSpan Span) : Statement(Span);

public record IfStatement(Expr Condition, Statement Then, Statement? Else, SourceSpan Span) : Statement(Span);

public record DefineSpaceStatement(Token Identifier, SourceSpan Span) : Statement(Span), IPrintableInformator
{
    public string GetInformation()
    {
        return Identifier.Value?.ToString() ?? "null";
    }
}

public record UndefineSpaceStatement(SourceSpan Span) : Statement(Span);