namespace TheParser.Cli.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RequirePositionalsAttribute(params string[] names) : Attribute
{
    public string[] Names => names;
}