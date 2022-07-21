using truename.Characteristics;

namespace truename;

public class Card
{
    // Id that never changes, probably not used for anything other
    // than tracking/storage purposes for us. maybe use scryfallId?
    public Guid Id { get; set; }

    // Id that defines the card in WotC Oracle db
    public Guid OracleId { get; set; }

    // Changes when object changes zones. Used for Aura's, Equipment, etc.
    public Guid ZonedId { get; set; }

    public ICharacteristic[] Characteristics { get; set; } = Array.Empty<ICharacteristic>();

    public Card(Guid oracleId, params ICharacteristic[] characteristics)
        : this(Guid.NewGuid(), Guid.NewGuid(), oracleId, characteristics)
    {
    }

    public Card(Guid id, Guid zonedId, Guid oracleId, params ICharacteristic[] characteristics)
    {
        Id = id;
        ZonedId = zonedId;
        OracleId = oracleId;
        Characteristics = characteristics;
    }

    public T? GetCharacteristic<T>() where T : ICharacteristic =>
        Characteristics
            .OfType<T>()
            .SingleOrDefault();

    public override string ToString() =>
        $"{GetCharacteristic<CardName>()?.Value} ({OracleId.ToString().Substring(0, 8)})";

    public Card ChangingZone() => new Card(
        Id,
        Guid.NewGuid(),
        OracleId,
        Characteristics
    );

    public static Card ChangedZone(Card card) => new Card(
        card.Id,
        Guid.NewGuid(),
        card.OracleId,
        card.Characteristics
    );
}