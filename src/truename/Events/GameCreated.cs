namespace truename.Events;

public record GameCreated(Guid Id, int Number, Player[] Players) : IGameEvent
{
  public string Name => "Game Created";

  public void Resolve(Game g)
  {
    g.Players = Players.ToDictionary(p => p.Id);
    g.Turns = Players.ToDictionary(p => p.Id, p => 0);
    g.TurnStep = "Determine-Turn-Order";
    g.Zones = new()
    {
      [(Zones.Battlefield, string.Empty)] = new(),
      [(Zones.Stack, string.Empty)] = new(),
      [(Zones.Exile, string.Empty)] = new()
    };

    g.Players.ForEach(p =>
    {
      var player = g.Players[p.Key];
      g.Zones[(Zones.Graveyard, p.Key)] = new();
      g.Zones[(Zones.Hand, p.Key)] = new();
      g.Zones[(Zones.Library, p.Key)] = new(player.DeckList);
    });
  }
}