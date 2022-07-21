namespace truename.Events;

public class PutIntoPlay : IGameEvent
{
    public string PlayerId { get; set; }
    public Guid ZonedId { get; set; }
    public (ZoneTypes, string?) From { get; set; }

    public PutIntoPlay(string playerId, Guid zonedId, (ZoneTypes, string?) from)
    {
        PlayerId = playerId;
        ZonedId = zonedId;
        From = from;
    }

    public GameEffect Resolve => (m, _, _, _) =>
    {
        var sourceZone = m.Zones[From];
        var battlefield = m.Zones[(ZoneTypes.Battlefield, null)];
        var card = sourceZone.Find(c => c.ZonedId == ZonedId);

        if (card == null)
            throw new Exception("Attempted to put into play a card that couldn't be found in it's source zone.");

        sourceZone.Remove(card);
        battlefield.Add(card.ChangingZone());
    };
}
