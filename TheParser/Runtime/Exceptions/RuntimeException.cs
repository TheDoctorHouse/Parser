
using TheParser.Debugging.Exceptions;

namespace TheParser.Runtime.Exceptions;

public abstract class RuntimeException(string message) : LanguageException(message);