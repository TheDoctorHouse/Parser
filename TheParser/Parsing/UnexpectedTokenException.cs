using TheParser.Lexing;
using System.Text;
using TheParser.Contracts;

namespace TheParser.Parsing;

public class UnexpectedTokenException : Exception
{
    private readonly StringBuilder _builder;

    public override string Message => _builder.ToString();
    public UnexpectedTokenException(Token unexpected) : this(unexpected, (ICodeStream?)null, null)
    {
    }

    public UnexpectedTokenException(Token unexpected, string message)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"This token was not expected: {unexpected}");
        sb.AppendLine($"Position: {unexpected.Position}");
        sb.AppendLine(message);

        _builder = sb;
    }

    public UnexpectedTokenException(Token unexpected, ICodeStream? codeStream, params TokenType[]? expected)
    { 
        var sb = new StringBuilder();

        sb.AppendLine($"This token was not expected here:");

        if (codeStream != null)
            unexpected.AppendDebugInfo(codeStream, sb);
        else
            sb.AppendLine(unexpected.ToString());

        if (expected?.Length > 0)
        {
            sb.AppendLine("Was expecting: ");

            for (int i = 0; i < expected.Length; i++)
            {
                sb.Append(expected[i]);
                if (i == expected.Length - 1)
                    sb.Append('.');
                else
                    sb.Append(',');
                    sb.Append(' ');
            }
        }
    
        _builder = sb;
    }

    public UnexpectedTokenException(Token unexpected, string message, params TokenType[] expected)
     : this(unexpected, (ICodeStream?)null, expected)
    {
        _builder.AppendLine(message);
    }
}