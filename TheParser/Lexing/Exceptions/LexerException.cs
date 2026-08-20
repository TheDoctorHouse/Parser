using TheParser.Debugging.Exceptions;

namespace TheParser.Lexing.Exceptions;

public abstract class LexerException(string message) : LanguageException(message);