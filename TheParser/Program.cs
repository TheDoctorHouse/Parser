using TheParser;

Console.Write("Code: ");
string content = Console.ReadLine() ?? throw new ArgumentException("Input is null.");


Console.WriteLine("Lexer:");
var tokenizer = new Lexer(content);
var token = tokenizer.NextToken();
while (token.TokenType != TokenType.EOF)
{
    if (token.Value != null)
        Console.Write($"{token.TokenType}({token.Value}) ");
    else
        Console.Write($"{token.TokenType} ");
    token = tokenizer.NextToken();
}

Console.Write(token.TokenType);

tokenizer.Reset();

Console.WriteLine("\nAst builder: ");
var parser = new Parser(tokenizer);

var statement = parser.Parse();

var printer = new AstPrinter();

string tree = printer.Print(statement);
Console.WriteLine(tree);

Console.WriteLine("Interpreter: ");
var interpreter = new Interpreter();

interpreter.InterpretStatement(statement);
Console.ReadLine();