using TheParser.Runtime.Functions.Attributes;

namespace TheParser.Runtime.Functions.BuiltIns;

[BuiltInFunction("Ask")]
public class Ask : BuiltInFunction
{
    public override Interpretation Invoke(IReadOnlyList<Interpretation> arguments)
    {
        string input = Console.ReadLine() ?? "Nothing.";
        return new StringInterpretation(input);
    }
}