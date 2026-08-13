using TheParser.Runtime.Functions.Attributes;

namespace TheParser.Runtime.Functions.BuiltIns;

[BuiltInFunction("ConvertToNumber", typeof(StringInterpretation))]
public class ConvertToNumber : BuiltInFunction
{
    public override Interpretation Invoke(IReadOnlyList<Interpretation> arguments)
    {
        var input = (StringInterpretation)arguments[0];

        if (!double.TryParse(input.Value, out double val))
            throw new InterpretationException($"Failed to parse integer.");

        return new NumberInterpretation(val);
    }
}