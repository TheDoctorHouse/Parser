using TheParser.Runtime.Functions.Attributes;
using TheParser.Runtime.IO;

namespace TheParser.Runtime.Functions.BuiltIns;

[BuiltInFunction("Print", typeof(IStringInterpretable))]
public class Print(IPrinter printer) : BuiltInFunction
{
    public override Interpretation Invoke(IReadOnlyList<Interpretation> arguments)
    {
        IStringInterpretable stringInterpretable = (IStringInterpretable)arguments[0];
        string output = stringInterpretable.InterpretToString().Value;

        printer.Print(output);
        return new NothingInterpretation();
    }
}