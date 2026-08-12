namespace TheParser.Contracts;

/// <summary>
/// Represents a seekable stream of source-code characters with access to
/// character positions and source-line information.
/// </summary>
public interface ICodeStream
{
    /// <summary>
    /// Gets the position of the current character.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// Gets the character at the current position.
    /// </summary>
    char? Current { get; }

    int Length { get; }

    /// <summary>
    /// Returns the next character without changing the current position,
    /// or <see langword="null"/> if there is no next character.
    /// </summary>
    char? Peek();

    /// <summary>
    /// Advances to the next character and returns it,
    /// or <see langword="null"/> if there is no next character.
    /// </summary>
    char? Next();

    /// <summary>
    /// Moves the stream to the specified position.
    /// </summary>
    void Seek(int position);

    int GetLineNumber(int position);
    int GetLineStart(int position);
    int GetLineEnd(int position);
    string GetLine(int position);
}