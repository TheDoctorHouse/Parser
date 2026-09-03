using System.Text;
using TheParser.Lexing.Exceptions;
using TheParser.Syntax;

namespace TheParser.Lexing;

public class Lexer
{
    public Token? Current { get; private set; }

    private string _content;
    public int Position { get; private set; }

    private const string TrueKeyword = "true";
    private const string FalseKeyword = "false";

    public Lexer(string input)
    {
        _content = input;
    }

    public Token NextToken()
    {
        var token = NextTokenInternal();
        Current = token;
        return token;
    }

    public Token Peek()
    {
        var position = Position;
        var token = NextTokenInternal();
        Position = position;
        return token;
    }

    public void Reset()
    {
        Position = 0;
    }

    private Token NextTokenInternal()
    {
        if (Position >= _content.Length)
            return CreateToken(TokenType.EOF);

        var currentChar = _content[Position];

        if (TryConsumeSymbolToken(currentChar, out Token? token))
            return token!;
        
        if (char.IsWhiteSpace(currentChar))
        {
            NextCharacter();
            return NextToken();
        }

        if (TryConsumeKeyword(TrueKeyword))
            return CreateToken(TokenType.Boolean, value: true);

        if (TryConsumeKeyword(FalseKeyword))
            return CreateToken(TokenType.Boolean, value: false);

        if (char.IsDigit(currentChar))
        {
            double value = ParseNumber(currentChar);
            return CreateToken(TokenType.Number, value);
        }

        if (IsIdentifierCharacter(currentChar))
        {
            string value = ParseIdentifier(currentChar);
            return CreateToken(TokenType.Identifier, value);
        }

        if (currentChar == '"')
        {
            string value = ParseString();
            return CreateToken(TokenType.String, value);
        }

        throw new UnexpectedCharacterException(
            $"Cannot resolve character '{currentChar}'.",
            CreateSpan()
            );
    }

    private bool TryConsumeSymbolToken(in char currentChar, out Token? token)
    {
        switch (currentChar)
        {
            case '+':
                NextCharacter();
                token = CreateToken(TokenType.Plus);
                break;
            case '-':
                NextCharacter();
                token = CreateToken(TokenType.Minus);
                break;
            case '*':
                NextCharacter();
                token = CreateToken(TokenType.Multiply);
                break;
            case '/':
                NextCharacter();
                token = CreateToken(TokenType.Divide);
                break;
            case '(':
                NextCharacter();
                token = CreateToken(TokenType.OpeningParentheses);
                break;
            case ')':
                NextCharacter();
                token = CreateToken(TokenType.ClosingParentheses);
                break;
            case ';':
                NextCharacter();
                token = CreateToken(TokenType.Semicolon);
                break;
            case ',':
                NextCharacter();
                token = CreateToken(TokenType.Comma);
                break;
            case '@':
                NextCharacter();
                token = CreateToken(TokenType.Declaration);
                break;
            case '=':
                NextCharacter();
                token = CreateToken(TokenType.Equals);
                break;
            default:
                token = default;
                return false;
        }

        return true;
    }

    private bool TryConsumeKeyword(string word)
    {
        int initialPos = Position;

        foreach (char c in word)
        {
            if (Position >= _content.Length || c != _content[Position])
            {
                Position = initialPos;
                return false;
            }

            Position++;
        }

        if (Position < _content.Length && IsIdentifierCharacter(_content[Position]))
        {
            Position = initialPos;
            return false;
        }

        return true;
    }

    private bool TryConsume(string value)
    {
        int initialPos = Position;

        foreach (var c in value)
        {
            if (Position >= _content.Length || c != _content[Position])
            {
                Position = initialPos;
                return false;
            }

            Position++;
        }

        return true;
    }

    public SourceSpan CreateSpan()
    {
        return new SourceSpan(Position, 1);
    }

    private static bool IsIdentifierCharacter(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9';

    private string ParseString()
    {
        int startingPosition = Position;
        char? currentChar = NextCharacter();
        StringBuilder sb = new();

        while (currentChar != null && currentChar.Value != '"')
        {
            // `\n` is currently the only escape sequence in the language. The more escape sequences will be added later.
            if (currentChar.Value == '\\' && TryPeek(out char c) && c == 'n')
            {
                sb.Append('\n');
                NextCharacter();
                currentChar = NextCharacter();
            }
            else
            {
                sb.Append(currentChar.Value);
                currentChar = NextCharacter();
            }
        }

        if (!currentChar.HasValue)
        {
            Position = startingPosition;
            throw new UnexpectedCharacterException("Expected `\"`, got end of file.", CreateSpan());
        }

        NextCharacter();

        return sb.ToString();
    }

    private bool TryPeek(out char c)
    {
        var peekPosition = Position + 1;
        var success = peekPosition < _content.Length;
        if (success)
        {
            c = _content[peekPosition];
            return true;
        }

        c = default;
        return false;
    }

    private string ParseIdentifier(char? currentChar)
    {
        StringBuilder sb = new StringBuilder();
        while (currentChar != null && IsIdentifierCharacter(currentChar.Value))
        {
            sb.Append(currentChar.Value);
            currentChar = NextCharacter();
        }

        return sb.ToString();
    }

    private Token CreateToken(TokenType type, object? value = null) =>
        new Token(type, value, Position);

    private char? NextCharacter()
    {
        Position += 1;
        if (Position >= _content.Length)
            return null;
        return _content[Position];
    }

    private double ParseNumber(char? currentChar)
    {
        if (currentChar == null)
            return 0;

        int value = currentChar.Value - '0';
        currentChar = NextCharacter();

        while (currentChar != null && char.IsDigit(currentChar.Value))
        {
            int number = currentChar.Value - '0';
            value *= 10;
            value += number;
            currentChar = NextCharacter();
        }

        return value;
    }
}
