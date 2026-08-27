namespace TheParser.Cli;

public class DependencyInjector
{
    private readonly Dictionary<Type, object> _dependencies;

    public DependencyInjector()
    {
        _dependencies = [];
        _dependencies.Add(typeof(DependencyInjector), this);
    }

    public void AddSingleton<T>(T dep) where T : notnull
    {
        if (!_dependencies.TryAdd(typeof(T), dep))
            throw new InvalidOperationException("Cannot add the same singleton type twice.");
    }

    private object ResolveOrThrow(Type t)
    {
        if (_dependencies.TryGetValue(t, out var dep))
            return dep;

        throw new InvalidOperationException($"Cannot resolve type `{t.FullName}`");
    }
    
    public object CreateInstance(Type t)
    {
        var constructors = t.GetConstructors();
        if (constructors.Length != 1)
        {
            throw new InvalidOperationException(
                $"Type `{t.FullName}` must have exactly one public constructor.");
        }
        var parameters = constructors[0].GetParameters();
        var args = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var instance = ResolveOrThrow(p.ParameterType);
            args[i] = instance;
        }

        return Activator.CreateInstance(t, args)!;
    }
}