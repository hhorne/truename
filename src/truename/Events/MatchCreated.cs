namespace truename.Events;

public class MatchCreated : IGameEvent
{
    public Guid Id { get; set; }
    public int PlayerCount { get; set; }
    public int BestOf { get; set; }

    public MatchCreated(Guid id, int playerCount, int bestOf)
    {
        Id = id;
        PlayerCount = playerCount;
        BestOf = bestOf;
    }

    public GameEffect Resolve => (match, _, _, _) =>
    {
        match.MatchId = Id;
        match.PlayerCount = PlayerCount;
        match.BestOf = BestOf;
    };
}
