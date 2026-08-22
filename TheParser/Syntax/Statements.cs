using TheParser.Lexing;

namespace TheParser.Syntax;

public abstract record Statement(SourceSpan Span);

public record BlockStatement(IReadOnlyList<Statement> Statements, SourceSpan Span) : Statement(Span);

public record ExpressionStatement(Expr Callee, SourceSpan Span) : Statement(Span);

public record VariableDeclarationStatement(Token Identifier, Expr? Initializer, SourceSpan Span) : Statement(Span);