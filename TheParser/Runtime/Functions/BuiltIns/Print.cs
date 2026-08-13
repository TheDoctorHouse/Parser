using TheParser.Runtime.Functions.Attributes;

namespace TheParser.Runtime.Functions.BuiltIns;

[BuiltInFunction("Print", typeof(IStringInterpretable))]
public class Print : BuiltInFunction
{
    public override Interpretation Invoke(IReadOnlyList<Interpretation> arguments)
    {
        IStringInterpretable stringInterpretable = (IStringInterpretable)arguments[0];
        string output = stringInterpretable.InterpretToString().Value;

        Console.Write(output);
        return new NothingInterpretation();
    }
}