namespace TheParser.Runtime;

public interface IStringInterpretable : IInterpretationConstraint
{
    StringInterpretation InterpretToString();
}

public interface IInterpretationConstraint;

public abstract record class Interpretation
{
    public Interpretation Interpret() => this;
}

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

public record class BooleanInterpretation(bool Value) : Interpretation, IStringInterpretable
{
    public StringInterpretation InterpretToString()
    {
        return new StringInterpretation(Value.ToString());
    }
}

public record class NullInterpretation : Interpretation;