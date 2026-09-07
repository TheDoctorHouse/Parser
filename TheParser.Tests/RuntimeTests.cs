using System.Reflection;
using TheParser.Lexing;
using TheParser.Parsing;
using TheParser.Runtime;
using TheParser.Runtime.Exceptions;
using TheParser.Runtime.Functions.Attributes;
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
    [InlineData("@foo; Print(foo);")]
    public void InterpretStatement_IncorrectArguments_ThrowsInvalidArgumentsException(string input)
    {
        Statement st = ParseStatement(input);
        Interpreter interpreter = new(TestUtility.CreateConsoleDependencyInjector());
        Assert.Throws<InvalidArgumentsException>(() =>
            interpreter.InterpretStatement(st));
    }

    private static Statement ParseStatement(string input)
    {
        Lexer lexer = new(input);
        Parser parser = new(lexer);

        return parser.ParseBlockStatement();
    }

}