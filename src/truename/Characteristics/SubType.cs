namespace truename.Characteristics;

public class SubType : Characteristic<string[]>
{
    public SubType(params string[] value) : base(value) { }
}
