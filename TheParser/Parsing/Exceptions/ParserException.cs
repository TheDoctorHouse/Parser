using TheParser.Debugging.Exceptions;

namespace TheParser.Parsing.Exceptions;

public abstract class ParserException(string message) : LanguageException(message);