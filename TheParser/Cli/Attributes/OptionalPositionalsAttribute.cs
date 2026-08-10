
namespace TheParser.Cli.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class OptionalPositionalsAttribute(params string[] names) : Attribute
{
    public string[] Names => names;
}