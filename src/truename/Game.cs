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
  public Dictionary<(ZoneKeys, Guid?), IEnumerable<Card>> Zones { get; set; } = new();

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
      [(ZoneKeys.Battlefield, null)] = Enumerable.Empty<Card>(),
      [(ZoneKeys.Stack, null)] = new Queue<Card>(),
      [(ZoneKeys.Exile, null)] = Enumerable.Empty<Card>()
    };

    Players.ForEach(p =>
    {
      var player = Players[p.Key];
      Zones[(ZoneKeys.Graveyard, p.Key)] = Enumerable.Empty<Card>();
      Zones[(ZoneKeys.Hand, p.Key)] = Enumerable.Empty<Card>();
      Zones[(ZoneKeys.Library, p.Key)] = new List<Card>(player.DeckList);
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

  public void UpdateZone((ZoneKeys, Guid?) zoneId, IEnumerable<Card> cards)
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
