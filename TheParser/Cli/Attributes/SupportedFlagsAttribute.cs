namespace TheParser.Cli.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SupportedFlagsAttribute(params string[] flags) : Attribute
{
    public string[] Flags => flags;
}