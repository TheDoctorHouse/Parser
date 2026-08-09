using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Debugging;
using TheParser.Runtime;

if (args.Length <= 1)
{
    Console.WriteLine("Usage: interpret <file>");

    return 1;
}

string path = args[1];

if (!File.Exists(path))
{
    Console.Error.WriteLine($"File not found: {path}");
    return 1;
}

bool debug = args.Contains("--debug");

string code = File.ReadAllText(path);

var tokenizer = new Lexer(code);

if (debug)
{
    var token = tokenizer.NextToken();
    Console.WriteLine("Lexer:");
    while (token.TokenType != TokenType.EOF)
    {
        if (token.Value != null)
            Console.Write($"{token.TokenType}({token.Value}) ");
        else
            Console.Write($"{token.TokenType} ");
        token = tokenizer.NextToken();
    }

    Console.Write(token.TokenType);
}

tokenizer.Reset();


var parser = new Parser(tokenizer);

var statement = parser.Parse();

if (debug)
{
    Console.WriteLine("\nAst builder: ");

    var printer = new AstPrinter();

    string tree = printer.Print(statement);
    Console.WriteLine(tree);
    Console.WriteLine("Interpreter: ");
}

var interpreter = new Interpreter();

interpreter.InterpretStatement(statement);
return 0;