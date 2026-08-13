using TheParser.Lexing;

namespace TheParser.Syntax;

public abstract record Statement;

public record BlockStatement(IReadOnlyList<Statement> Statements) : Statement;

public record ExpressionStatement(Expr Callee) : Statement;

public record VariableDeclarationStatement(Token Identifier, Expr? Initializer) : Statement;