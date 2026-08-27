using TheParser.Runtime.Functions.Attributes;
using TheParser.Runtime.IO;

namespace TheParser.Runtime.Functions.BuiltIns;

[BuiltInFunction("Ask")]
public class Ask(IReader reader) : BuiltInFunction
{
    public override Interpretation Invoke(IReadOnlyList<Interpretation> arguments)
    {
        string input = reader.ReadLine() ?? "Nothing.";
        return new StringInterpretation(input);
    }
}