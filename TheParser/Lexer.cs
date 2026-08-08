using System.Text;

namespace TheParser;

public class Lexer
{
    public int CurrentPosition { get; private set; }

    public Token? Current { get; private set; }

    public string Content => _content;
    
    private readonly string _content;

    public Lexer(string content)
    {
        _content = content;
    }

    public Token NextToken()
    {
        var token = NextTokenInternal();
        Current = token;
        return token;
    }

    public Token Peek()
    {
        var position = CurrentPosition;
        var token = NextTokenInternal();
        CurrentPosition = position;
        return token;
    }

    public void Reset()
    {
        CurrentPosition = 0;
    }

    private Token NextTokenInternal()
    {
        if (CurrentPosition >= _content.Length)
        {
            return CreateToken(TokenType.EOF);
        }

        var currentChar = _content[CurrentPosition];

        switch (currentChar)
        {
            case '+':
                NextCharacter();
                return CreateToken(TokenType.Plus);
            case '-':
                NextCharacter();
                return CreateToken(TokenType.Minus);
            case '*':
                NextCharacter();
                return CreateToken(TokenType.Multiply);
            case '/':
                NextCharacter();
                return CreateToken(TokenType.Divide);
            case '(':
                NextCharacter();
                return CreateToken(TokenType.OpeningParentheses);
            case ')':
                NextCharacter();
                return CreateToken(TokenType.ClosingParentheses);
            case ';':
                NextCharacter();
                return CreateToken(TokenType.Semicolon);
            case ',':
                NextCharacter();
                return CreateToken(TokenType.Comma);
            case '@':
                NextCharacter();
                return CreateToken(TokenType.Declaration);
            case '=':
                NextCharacter();
                return CreateToken(TokenType.Equals);
            default:
                break;
        }

        if (char.IsWhiteSpace(currentChar))
        {
            NextCharacter();
            return NextToken();
        }

        if (char.IsDigit(currentChar))
        {
            double value = ParseNumber(currentChar);
            return CreateToken(TokenType.Number, value);
        }

        if (IsLetter(currentChar))
        {
            string value = ParseIdentifier(currentChar);
            return CreateToken(TokenType.Identifier, value);
        }

        if (currentChar == '"')
        {
            string value = ParseString();
            return CreateToken(TokenType.String, value);
        }


        throw UnexpectedCharacterException(currentChar);
    }

    private static bool IsLetter(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';

    private string ParseString()
    {
        int startingPosition = CurrentPosition;
        char? currentChar = NextCharacter();
        StringBuilder sb = new StringBuilder();
        
        while (currentChar != null && currentChar.Value != '"')
        {
            if (currentChar.Value == '\\' 
            && CurrentPosition + 1 != _content.Length 
            && _content[CurrentPosition + 1] == 'n')
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
            CurrentPosition = startingPosition;
            throw Failure("Expected `\"`, got end of file.");
        }

        NextCharacter();

        return sb.ToString();
    }

    private string ParseIdentifier(char? currentChar)
    {
        StringBuilder sb = new StringBuilder();
        while (currentChar != null && IsLetter(currentChar.Value))
        {
            sb.Append(currentChar.Value);
            currentChar = NextCharacter();
        }

        return sb.ToString();
    }

    private Exception UnexpectedCharacterException(char character)
    {
        return Failure($"Cannot resolve character '{character}', position {CurrentPosition}");
    }

    private Exception Failure(string message)
    {
        string visualization = DebugUtility.PingPosition(CurrentPosition, _content);
        return new Exception(message + "\n" + visualization);
    }
    private Token CreateToken(TokenType type, object? value = null) => 
        new Token(type, value, CurrentPosition);

    private char? NextCharacter()
    {
        CurrentPosition++;
        if (IsEndOfContent)
            return null;

        return _content[CurrentPosition];
    }

    public bool IsEndOfContent => CurrentPosition >= _content.Length; 

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