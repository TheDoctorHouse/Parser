namespace TheParser.Lexing;

public enum TokenType
{
    Plus,
    Minus,
    Multiply,
    Divide,
    Number,
    String,
    Boolean,
    Equals,
    EqualsEquals,
    LessEqual,
    GreaterEqual,
    NotEqual,
    Greater,
    Less,
    Comma,
    Semicolon,
    Separator,
    EOF,
    OpeningParentheses,
    ClosingParentheses,
    If,
    Else,
    OpeningBrace,
    ClosingBrace,
    Identifier,
    Declaration,
    DefineSpace,
    UndefineSpace
}