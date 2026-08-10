namespace TheParser.Cli.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandNameAttribute(string commandName) : Attribute
{
    public string CommandName => commandName;
};