using System.Collections.Immutable;

namespace TheParser.Runtime.Functions;

public interface IFunction
{
    public IReadOnlyList<Type> GetParameterTypes();
    string Name { get; }

    Interpretation Invoke(IReadOnlyList<Interpretation> arguments);
}