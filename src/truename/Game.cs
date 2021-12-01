using truename.Domain;
using truename.Effects;
using truename.Events;

namespace truename;

using ZoneKeys = truename.Zones;

public partial class Game : AggregateRoot
{
  public int Number { get; set; }
  public List<string> TurnOrder { get; set; } = new();
  public Dictionary<string, Player> Players { get; set; } = new();
  public string ActivePlayerId { get; set; } = string.Empty;
  public Player ActivePlayer => Players[ActivePlayerId];
  public string PriorityHolderId { get; set; } = string.Empty;
  public Dictionary<(ZoneKeys, string), List<Card>> Zones { get; set; } = new();
  public List<GameEvent> EventLog { get; set; } = new();
  public Dictionary<string, int> Turns { get; set; } = new();
  public string TurnStep { get; set; } = string.Empty;
  public List<ContinuousEffect> ContinuousEffects { get; set; } = new();

  public Game() { }

  public Game(Player[] players)
      : this(Guid.NewGuid(), 1, players)
  {
  }

  public Game(int number, Player[] players)
      : this(Guid.NewGuid(), number, players)
  {
  }

  public Game(Guid id, int number, Player[] players)
  {
    var @event = new GameCreated(id, number, players);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(GameCreated @event)
  {
    Id = @event.Id;
    Players = @event.Players.ToDictionary(o => o.Id);

    Zones = new()
    {
      [(ZoneKeys.Battlefield, string.Empty)] = new(),
      [(ZoneKeys.Stack, string.Empty)] = new(),
      [(ZoneKeys.Exile, string.Empty)] = new()
    };

    Players.ForEach(p =>
    {
      var player = Players[p.Key];
      Zones[(ZoneKeys.Graveyard, p.Key)] = new();
      Zones[(ZoneKeys.Hand, p.Key)] = new();
      Zones[(ZoneKeys.Library, p.Key)] = new(player.DeckList);
    });
  }

  public void SetTurnOrder(IEnumerable<string> turnOrder)
  {
    var @event = new SetTurnOrder(turnOrder);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(SetTurnOrder @event)
  {
    TurnOrder = @event.TurnOrder.ToList();
    Turns = TurnOrder.ToDictionary(p => p, p => 1);
    ActivePlayerId = TurnOrder.First();
  }

  public void UpdateZone((ZoneKeys, string) zoneId, IEnumerable<Card> cards)
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
    TurnStep = @event.TurnStep;
  }

  public GameEvent Log(GameEvent gameEvent)
  {
    var @event = new LogGameEvent(gameEvent);
    Apply(@event);
    AddUncommittedEvent(@event);
    return gameEvent;
  }

  public void Apply(LogGameEvent @event)
  {
    EventLog.Add(@event.GameEvent);
  }
}
