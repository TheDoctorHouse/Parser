using TheParser.Debugging;
using System.Text;
using TheParser.Contracts;

namespace TheParser.Lexing;

public class Lexer
{
    public Token? Current { get; private set; }

    public ICodeStream _codeStream;

    public Lexer(ICodeStream codeStream)
    {
        _codeStream = codeStream;
    }

    public Token NextToken()
    {
        var token = NextTokenInternal();
        Current = token;
        return token;
    }

    public Token Peek()
    {
        var position = _codeStream.Position;
        var token = NextTokenInternal();
        _codeStream.Seek(position);
        return token;
    }

    public void Reset()
    {
        _codeStream.Seek(0);
    }

    private Token NextTokenInternal()
    {
        if (_codeStream.Current == null)
            return CreateToken(TokenType.EOF);

        var currentChar = _codeStream.Current.Value;

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
        int startingPosition = _codeStream.Position;
        char? currentChar = NextCharacter();
        StringBuilder sb = new ();
        
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
            _codeStream.Seek(startingPosition);
            throw Failure("Expected `\"`, got end of file.");
        }

        NextCharacter();

        return sb.ToString();
    }

    private bool TryPeek(out char c)
    {
        char? result = _codeStream.Peek();

        c = default;

        if (result is null)
            return false;

        c = result.Value;
        return true;
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
        return Failure($"Cannot resolve character '{character}', position {_codeStream.Current}");
    }

    private Exception Failure(string message)
    {
        string visualization = DebugUtility.PingPosition(_codeStream.Position, _codeStream);
        return new Exception(message + "\n" + visualization);
    }
    private Token CreateToken(TokenType type, object? value = null) => 
        new Token(type, value, _codeStream.Position);

    private char? NextCharacter()
    {
        return _codeStream.Next();
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
