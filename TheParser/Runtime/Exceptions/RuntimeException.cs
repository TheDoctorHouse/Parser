
using TheParser.Debugging.Exceptions;
using TheParser.Syntax;

namespace TheParser.Runtime.Exceptions;

public abstract class RuntimeException(string message, SourceSpan span) : LanguageException(message, span);