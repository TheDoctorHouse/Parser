using System.Collections.Immutable;

namespace TheParser.Runtime.Functions;

public abstract class BuiltInFunction
{
    public abstract Interpretation Invoke(IReadOnlyList<Interpretation> arguments);
}

internal class BuiltInFunctionAdapter(string name, Type[] parameters, BuiltInFunction function) : IFunction
{
    public string Name => name;

    public IReadOnlyList<Type> GetParameterTypes() => parameters;

    public Interpretation Invoke(IReadOnlyList<Interpretation> arguments) => function.Invoke(arguments);
}