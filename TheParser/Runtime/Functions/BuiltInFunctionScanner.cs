using System.Reflection;
using TheParser.Cli;
using TheParser.Runtime.Functions.Attributes;

namespace TheParser.Runtime.Functions;

public static class BuiltInFunctionScanner
{
    public static Dictionary<string, IFunction> ScanAndCreateBuiltInFunctions(DependencyInjector injector)
    {
        var functionTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => !t.IsAbstract && typeof(BuiltInFunction).IsAssignableFrom(t));

        Dictionary<string, IFunction> functions = [];

        foreach (var funcType in functionTypes)
        {
            var attribute = funcType.GetCustomAttribute<BuiltInFunctionAttribute>() ??
                throw new InvalidOperationException(
                    $"Built in function '{funcType.FullName}' is missing {nameof(BuiltInFunctionAttribute)}."
                    );

            for (int i = 0; i < attribute.Arguments.Length; i++)
            {
                Type? t = attribute.Arguments[i];
                bool assignableToIntepretation = typeof(Interpretation).IsAssignableFrom(t);
                bool assignableToInterpretationConstraint = t.IsInterface
                    && typeof(IInterpretationConstraint).IsAssignableFrom(t);

                if (!assignableToIntepretation && !assignableToInterpretationConstraint)
                    throw new InvalidOperationException(
                        $"Built in function '{funcType.FullName}' parameter {i + 1} is not an interpretation type or an interpretation constraint."
                        );
            }

            var builtIn = (BuiltInFunction)injector.CreateInstance(funcType)!;
            var adapter = new BuiltInFunctionAdapter(attribute.Name, attribute.Arguments, builtIn);

            functions.Add(attribute.Name, adapter);
        }

        return functions;
    }
}