namespace truename.Characteristics;

public class PowerAndToughness : Characteristic<(int,int)>
{
    public PowerAndToughness((int,int) value) : base(value) { }
}
