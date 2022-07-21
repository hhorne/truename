namespace truename.Events;

public class BottomCard : IGameEvent
{
    public Guid ZonedId { get; }
    public string PlayerId { get; }
    public (ZoneTypes, string?) From { get; }
    
    public BottomCard(Guid zonedId, string playerId, (ZoneTypes, string?) from)
    {
        ZonedId = zonedId;
        PlayerId = playerId;
        From = from;
    }

    public GameEffect Resolve => (m, _, _, _) =>
    {
        var sourceZone = m.Zones[From];
        var library = m.Zones[(ZoneTypes.Library, PlayerId)];
        var card = sourceZone.Single(c => c.ZonedId == ZonedId);
        sourceZone.Remove(card);
        library.Insert(0, card.ChangingZone());
    };
}
