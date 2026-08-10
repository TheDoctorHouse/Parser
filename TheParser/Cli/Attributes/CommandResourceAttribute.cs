namespace TheParser.Cli.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandResourceAttribute(Resource resource) : Attribute
{
    public Resource Resource => resource;
};