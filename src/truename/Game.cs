using truename.Domain;
using truename.Events;

namespace truename;

using ZoneKeys = truename.Zones;

public partial class Game : AggregateRoot
{
  public int Number { get; set; }
  public List<Guid> TurnOrder { get; set; } = new();
  public Dictionary<Guid, Player> Players { get; set; } = new();
  public Guid? ActivePlayerId { get; set; } = null;
  public Dictionary<(ZoneKeys, Guid?), IEnumerable<string>> Zones { get; set; } = new();

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
      [(ZoneKeys.Battlefield, null)] = Enumerable.Empty<string>(),
      [(ZoneKeys.Stack, null)] = new Queue<string>(),
      [(ZoneKeys.Exile, null)] = Enumerable.Empty<string>()
    };

    Players.ForEach(p =>
    {
      var player = Players[p.Key];
      Zones[(ZoneKeys.Graveyard, p.Key)] = Enumerable.Empty<string>();
      Zones[(ZoneKeys.Hand, p.Key)] = Enumerable.Empty<string>();
      Zones[(ZoneKeys.Library, p.Key)] = new List<string>(player.DeckList);
    });
  }

  public void SetTurnOrder(IEnumerable<Guid> turnOrder)
  {
    var @event = new SetTurnOrder(turnOrder);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(SetTurnOrder @event)
  {
    TurnOrder = @event.TurnOrder.ToList();
    ActivePlayerId = TurnOrder.First();
  }

  public void UpdateZone((ZoneKeys, Guid?) zoneId, IEnumerable<string> cards)
  {
    var @event = new UpdateZone(zoneId, cards);
    Apply(@event);
    AddUncommittedEvent(@event);
  }

  public void Apply(UpdateZone @event)
  {
    Zones[@event.ZoneId] = @event.Cards;
  }
}
