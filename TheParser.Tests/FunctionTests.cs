using System.Diagnostics;
using System.Reflection;
using TheParser.Runtime;
using TheParser.Runtime.Functions;
using TheParser.Runtime.Functions.Attributes;

namespace TheParser.Tests;

public class RuntimeTests
{
    private static IEnumerable<Type> GetBuiltInFunctionTypes()
    {
        var assembly = typeof(BuiltInFunction).Assembly;

        var functions = assembly.GetTypes().Where(
            t => !t.IsAbstract &&
            typeof(BuiltInFunction).IsAssignableFrom(t));

        return functions;
    }


    [Fact]
    public void BuiltInFunctions_ArgumentsAssignableToInterpretationOrInterpretationConstraintTypes()
    {
        var funcTypes = GetBuiltInFunctionTypes();

        foreach (var f in funcTypes)
        {
            var attr = f.GetCustomAttribute<BuiltInFunctionAttribute>();
            Assert.NotNull(attr);
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
}