namespace truename.Characteristics;

public class SuperType : Characteristic<string[]>
{
    public SuperType(params string[] value) : base(value) { }
}
