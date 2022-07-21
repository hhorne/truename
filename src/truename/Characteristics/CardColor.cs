namespace truename.Characteristics;

using ColorCode = Char;

public class CardColor : Characteristic<ColorCode[]>
{
    public CardColor(params ColorCode[] value) : base(value) { }
}
