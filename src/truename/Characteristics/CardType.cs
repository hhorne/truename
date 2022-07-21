namespace truename.Characteristics;

public class CardType  : Characteristic<string[]>
{
    public CardType(params string[] value) : base(value) { }
}
