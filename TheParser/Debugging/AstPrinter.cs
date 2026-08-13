using System.Text;
using TheParser.Syntax;

namespace TheParser.Debugging;

public class AstPrinter
{
    private readonly StringBuilder _sb = new();
    private int _indentationLength;
    private int _depth;

    public string Print(Statement st, int indentationLength = 3)
    {
        _indentationLength = indentationLength;
        HandleStatement(st);
        return _sb.ToString();
    }

    private void HandleStatement(Statement statement, int depthIncrement = 1)
    {
        _depth += depthIncrement;
        HandlePersonality(statement);

        switch (statement)
        {
            case BlockStatement bs:
                AppendMessage("Statements");
                foreach (var st in bs.Statements)
                    HandleStatement(st, 2);

                break;
            case ExpressionStatement es:
                AppendMessage("Callee");
                HandleExpression(es.Callee, 2);
                break;
            case VariableDeclarationStatement vds:
                AppendMessage($"Identifier ({vds.Identifier.Value!})");
                if (vds.Initializer != null)
                {
                    AppendMessage("Initializer");
                    HandleExpression(vds.Initializer, 2);
                }
                else
                    AppendMessage("No initializer");
                break;
            default:
                throw new InvalidOperationException($"Unexpected statement type: `{statement.GetType().FullName}`");
        }

        _depth -= depthIncrement;
    }

    private void HandleExpression(Expr ast, int depthIncrement = 1)
    {
        _depth += depthIncrement;
        HandlePersonality(ast);

        switch (ast)
        {
            case NumberExpression:
            case StringExpression:
            case IdentifierExpression:
                break;
            case BinaryExpression be:
                AppendMessage("Left");

                HandleExpression(be.Left, 2);

                AppendMessage("Right");

                HandleExpression(be.Right, 2);

                break;
            case UnaryExpression ue:
                HandleExpression(ue.Expr);
                break;
            case CallExpression ce:
                AppendMessage("Callee");

                HandleExpression(ce.Callee, 2);

                AppendMessage("Arguments");

                foreach (Expr arg in ce.Arguments)
                    HandleExpression(arg, 2);

                break;
            default:
                throw new InvalidOperationException($"Unexpected expression type: `{ast.GetType().FullName}`");
        }
        _depth -= depthIncrement;
    }

    private void AppendMessage(string msg, int depthIncrement = 1)
    {
        _depth += depthIncrement;
        AppendIndentations();
        _depth -= depthIncrement;
        _sb.Append(' ');
        _sb.Append(msg);
    }

    private void HandlePersonality<T>(T person)
        where T : notnull
    {
        AppendIndentations();
        if (_depth > 0)
            _sb.Append(' ');

        _sb.Append(person.GetType().Name);

        if (person is IPrintableInformator printable)
        {
            _sb.Append(' ');
            _sb.Append($"({printable.GetInformation()})");
        }
    }

    private void AppendIndentations()
    {
        _sb.AppendLine();
        for (int i = 0; i < _depth * _indentationLength; i++)
        {
            _sb.Append('-');
        }
    }
}