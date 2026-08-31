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
    Comma,
    Semicolon,
    EOF,
    OpeningParentheses,
    ClosingParentheses,
    OpeningBrace,
    ClosingBrace,
    Identifier,
    Declaration,
}