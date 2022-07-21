using truename.Effects;
using truename.Events;

namespace truename;

using CardId = Guid;
using MatchId = Guid;
using PlayerId = String;
using GameStepId = String;
using GameNumber = Int32;
using LifePoints = Int32;

public delegate void GameEffect(
    Match match,
    CardId sourceId = new(),
    PlayerId controllerId = "",
    PlayerId ownerId = ""
);

public delegate bool GameCondition(
    Match match,
    CardId sourceId = new(),
    PlayerId controllerId = "",
    PlayerId ownerId = ""
);

public class Match
{
    public MatchId MatchId { get; set; }
    public int PlayerCount { get; set; }
    public int BestOf { get; set; }
    public GameNumber GameNumber { get; set; }
    public Dictionary<PlayerId, Player> Players { get; set; } = new();
    public PlayerId[] TurnOrder { get; set; } = Array.Empty<string>();
    public PlayerId ActivePlayerId { get; set; } = string.Empty;
    public GameStepId CurrentStep { get; set; } = Turn.PreGame.WaitingForPlayers;
    public Dictionary<PlayerId, LifePoints> LifeTotals { get;set; } = new();
    public Dictionary<(ZoneTypes, PlayerId?), List<Card>> Zones { get; set; } = new();
    public List<ContinuousEffect> ContinuousEffects { get; set; } = new();
    public List<IGameEvent> EventLog { get; set; } = new();
    public Dictionary<GameNumber, PlayerId?> Results { get; set; } = new();

    public Match(MatchId id, int playerCount = 2, int bestOf = 3)
    {
        Apply(new MatchCreated(id, playerCount, bestOf));
    }

    public void Apply(IGameEvent @event)
    {
        @event.Resolve(this);
        EventLog.Add(@event);
    }
}