using TheParser.Syntax;

namespace TheParser.Debugging.Exceptions;

public abstract class LanguageException(string message, SourceSpan span) : Exception(message)
{
    public SourceSpan Span { get; } = span;
}
