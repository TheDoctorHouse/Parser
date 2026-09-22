using TheParser.Runtime.Functions;

namespace TheParser.Runtime;

public record class Space(string Name)
{
    public Space(
        string name,
        Dictionary<string, Interpretation> variables,
        Dictionary<string, IFunction> functions) : this(name)
    {
        _variables = variables;
        _functions = functions;
    }

    private readonly Dictionary<string, Interpretation> _variables = [];
    private readonly Dictionary<string, IFunction> _functions = [];

    public bool TryAddVariable(string name, Interpretation variable) =>
        _variables.TryAdd(name, variable);

    public bool TryAddFunction(string name, IFunction function) =>
        _functions.TryAdd(name, function);

    public bool TryGetVariable(string name, out Interpretation? variable) =>
        _variables.TryGetValue(name, out variable);

    public bool TryGetFunction(string name, out IFunction? function) =>
        _functions.TryGetValue(name, out function);
}