namespace TheParser.Runtime;

public interface IStringInterpretable
{
    StringInterpretation InterpretToString();
}

public abstract record class Interpretation;

public record class NothingInterpretation : Interpretation, IStringInterpretable
{
    public StringInterpretation InterpretToString()
    {
        return new StringInterpretation("Nothing.");
    }
}

public record class StringInterpretation(string Value) : Interpretation, IStringInterpretable
{
    public StringInterpretation InterpretToString()
    {
        return new StringInterpretation(Value);
    }
}

public record class NumberInterpretation(double Value) : Interpretation, IStringInterpretable
{
    public StringInterpretation InterpretToString()
    {
        return new StringInterpretation(Value.ToString());
    }
}

public record class NullInterpretation : Interpretation;