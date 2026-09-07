using System.Diagnostics;
using System.Reflection;
using TheParser.Cli.IO;
using TheParser.DependencyInjection;
using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Runtime;
using TheParser.Runtime.Exceptions;
using TheParser.Runtime.Functions.Attributes;
using TheParser.Runtime.Functions.BuiltIns;
using TheParser.Runtime.IO;
using TheParser.Syntax;

namespace TheParser.Tests;

public class RuntimeTests
{
    [Fact]
    public void BuiltInFunctions_ArgumentsAssignableToInterpretationOrInterpretationConstraintTypes()
    {
        var funcTypes = TestUtility.GetBuiltInFunctionTypes();

        foreach (var f in funcTypes)
        {
            var attr = f.GetCustomAttribute<BuiltInFunctionAttribute>();
            Assert.True(attr is not null, $"Problem with {f.FullName}");
            foreach (var arg in attr.Arguments)
            {
                bool assignableToIntepretation = typeof(Interpretation).IsAssignableFrom(arg);
                bool assignableToInterpretationConstraint =
                    typeof(IInterpretationConstraint).IsAssignableFrom(arg) &&
                    arg.IsInterface;

                Assert.True(assignableToIntepretation || assignableToInterpretationConstraint);
            }
        }
    }

    [Theory]
    [InlineData("@someVar; Print(someMissingVar);")]
    [InlineData("Print(foo); @foo = \"Bar\";")]
    [InlineData("@foo = \"123\"; @bar = ConvertToNumber(foO);")]
    public void InterpretStatement_MissingVariable_ThrowsUnresolvedVariableException(string input)
    {
        Statement st = ParseStatement(input);
        Interpreter interpreter = new(TestUtility.CreateConsoleDependencyInjector());
        Assert.Throws<UnresolvedVariableException>(() =>
            interpreter.InterpretStatement(st));
    }

    [Theory]
    [InlineData("@bar = 123; Foo(something);")]
    [InlineData("print(\"Foo\");")]
    public void InterpretStatement_NonExistingFunctionCall_ThrowsUnresolvedFunctionException(string input)
    {
        Statement st = ParseStatement(input);
        Interpreter interpreter = new(TestUtility.CreateConsoleDependencyInjector());
        Assert.Throws<UnresolvedFunctionException>(() =>
            interpreter.InterpretStatement(st));
    }

    [Theory]
    [InlineData("@bar = 123; Print();")]
    [InlineData("@foo; @bar; Print(foo, bar);")]
    public void InterpretStatement_IncorrectArguments_ThrowsInvalidArgumentsException(string input)
    {
        Statement st = ParseStatement(input);
        Interpreter interpreter = new(TestUtility.CreateConsoleDependencyInjector());
        Assert.Throws<InvalidArgumentsException>(() =>
            interpreter.InterpretStatement(st));
    }

    [Theory]
    [InlineData("Print(2 > 3);", "False")]
    [InlineData("Print(2 == 2);", "True")]
    [InlineData("Print(false == (2 == 2));", "False")]
    [InlineData("@a = 213 == 213; @b = false; Print(a != b);", "True")]
    [InlineData("@a = \"something\"; @b = \"Something\"; Print(a == b);", "False")]
    [InlineData("@a = \"something\"; @b = \"something\"; Print(a == b);", "True")]
    public void InterpretStatement_ValidComparison_ReturnsCorrectOutput(string input, string expectedOutput)
    {
        var printer = new TestPrinter();
        Statement st = ParseStatement(input);
        var di = new DependencyInjector();
        di.AddSingleton<IPrinter>(printer);
        di.AddSingleton<IReader>(new TestReader());
        var interpreter = new Interpreter(di);
        interpreter.InterpretStatement(st);

        Assert.True(printer.TryDequeue(out string? output));
        Assert.Equal(expectedOutput, output);
    }

    [Theory]
    [InlineData("@a; a == 123;")]
    [InlineData("@a; 123 == a;")]
    [InlineData("Print(\"Yeah!\" >= 123);")]
    [InlineData("Print(\"yeah!\" >= \"yeah!\");")]
    public void InterpretStatement_InvalidComparison_ThrowsOperationInterpretationException(string input)
    {
        Statement st = ParseStatement(input);
        Interpreter interpreter = new(TestUtility.CreateConsoleDependencyInjector());
        Assert.Throws<OperationInterpretationException>(() =>
            interpreter.InterpretStatement(st));
    }

    private static Statement ParseStatement(string input)
    {
        Lexer lexer = new(input);
        Parser parser = new(lexer);

        return parser.ParseBlockStatement();
    }

}