namespace truename.Events;

public class Draw : IGameEvent
{
    public string PlayerId { get; set; }

    public Draw(string playerId)
    {
        PlayerId = playerId;
    }

    public GameEffect Resolve => (m, _, _, _) =>
    {
        var hand = m.Zones[(ZoneTypes.Hand, PlayerId)];
        var library = m.Zones[(ZoneTypes.Library, PlayerId)];
        var card = library.Last();
        library.Remove(card);
        hand.Add(card.ChangingZone());
    };
}