namespace truename.Events;

public class ShuffleLibrary : IGameEvent
{
    public string PlayerId { get; }
    
    public ShuffleLibrary(string playerId)
    {
        PlayerId = playerId;
    }

    public GameEffect Resolve => (m, _, _, _) =>
    {
        var players = m.Players;
        var player = players[PlayerId];
        var zones = m.Zones;
        var library = zones[(ZoneTypes.Library, PlayerId)];

        library = library
            .ToArray()
            .Shuffle()
            .ToList();
    };
}
