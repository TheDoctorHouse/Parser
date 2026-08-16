namespace TheParser.Lexing.Exceptions;

public class UnexpectedCharacterException(string message) : LexerException(message);