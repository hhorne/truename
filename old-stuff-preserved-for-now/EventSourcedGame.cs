using truename.Domain;
using truename.Effects;
using truename.Events;

namespace truename;

public partial class EventSourcedGame : AggregateRoot
{
  public int Number { get; set; }
  public Dictionary<string, Player> Players { get; set; } = new();
  public List<string> TurnOrder { get; set; } = new();
  public string ActivePlayerId { get; set; } = string.Empty;
  public Player ActivePlayer => Players[ActivePlayerId];
  public string PriorityHolderId { get; set; } = string.Empty;
  public Dictionary<(ZoneTypes, string), List<Card>> Zones { get; set; } = new();
  
  // Tracks all events through the course of the game, useful for Storm Count
  // and Effects like "Skip draw except for first draw of turn"
  //
  // Key is (playerId, turnNumber, turnStep)
  public Dictionary<(string, int, string), List<IGameEventOld>> Events { get; set; } = new();

  // Tracks the current Turn Number of each player
  public Dictionary<string, int> Turns { get; set; } = new();

  // Tracks modifications to normal flow of turn. Extra Turns, Phases and Steps
  public List<(string, List<string>)> TurnStack = new();

  // Current Turn Step
  public string TurnStep { get; set; } = "Game-Created";
  public List<ContinuousEffect> ContinuousEffects { get; set; } = new();

  public EventSourcedGame() { }

  public EventSourcedGame(Player[] players)
      : this(Guid.NewGuid(), 1, players)
  {
  }

  public EventSourcedGame(int number, Player[] players)
      : this(Guid.NewGuid(), number, players)
  {
  }

  public EventSourcedGame(Guid id, int number, Player[] players)
  {
    var @event = new GameCreated(id, number, players);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(GameCreated @event)
  {
    Id = @event.Id;
    @event.Resolve(this);
  }

  public void Apply(SetTurnOrder @event)
  {
    TurnOrder = @event.TurnOrder.ToList();
    ActivePlayerId = TurnOrder.First();
    //Log(@event);
  }

  public void UpdateZone((ZoneTypes, string) zoneId, IEnumerable<Card> cards)
  {
    var @event = new UpdateZone(zoneId, cards);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(UpdateZone @event)
  {
    Zones[@event.ZoneId] = new(@event.Cards);
  }

  public void UpdateActivePlayer(string playerId)
  {
    var @event = new UpdateActivePlayer(playerId);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(UpdateActivePlayer @event)
  {
    ActivePlayerId = @event.PlayerId;
    Turns[ActivePlayerId]++;
  }

  public void GivePriorityTo(string playerId)
  {
    var @event = new PassPriority(playerId);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(PassPriority @event)
  {
    PriorityHolderId = @event.PlayerId;
  }

  public void UpdateTurnStep(string turnStep)
  {
    var @event = new UpdateTurnStep(turnStep);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(UpdateTurnStep @event)
  {
    @event.Resolve(this);
  }

  public truename.IGameEventOld Log(truename.IGameEventOld gameEvent)
  {
    var key = (ActivePlayerId, Turns[ActivePlayerId], TurnStep);
    var @event = new LogEvent(key, gameEvent);
    Apply(@event);
    AddUncommittedEvent(@event);
    return gameEvent;
  }

  public void Apply(LogEvent @event)
  {
    if (!Events.ContainsKey(@event.Key))
    {
      Events[@event.Key] = new();
    }

    Events[@event.Key].Add(@event.GameEvent);
  }

  public void CreateContinuousEffect(ContinuousEffect effect)
  {
    var @event = new CreateContinuousEffect(effect);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(CreateContinuousEffect @event)
  {
    ContinuousEffects.Add(@event.Effect);
  }

  public void Draw(string playerId)
  {
    var @event = new Draw(playerId);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(Draw @event)
  {
    @event.Resolve(this);
  }
}
