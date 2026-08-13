namespace TheParser.Runtime.Functions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BuiltInFunctionAttribute(string name, params Type[] arguments) : Attribute
{
    public string Name => name;
    public Type[] Arguments => arguments;
}