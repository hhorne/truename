namespace truename.Systems;

public class MulliganSystem
{
  int DefaultHandSize = 7;
  private readonly Game game;
  private Dictionary<string, MulliganDecision> decisions = new();
  public Dictionary<string, MulliganDecision> Decisions => decisions;

  public IEnumerable<string> StillDeciding => decisions
    .Where(d => !d.Value.Keep)
    .Select(d => d.Key);

  public MulliganSystem(Game game)
  {
    this.game = game;
  }

  public void Init()
  {
    decisions = game
      .TurnOrder
      .ToDictionary(
        p => p,
        p => new MulliganDecision()
      );
  }

  public IEnumerable<GameEvent> DeclareMulligans()
  {
    foreach (var @event in KeepOrMull())
      yield return @event;

    foreach (var @event in PutCardsBack())
      yield return @event;
  }

  public IEnumerable<GameEvent> KeepOrMull()
  {
    var choices = StillDeciding
      .Select(pId => new GameEvent
      {
        Name = $"{game.GetPlayerName(pId)}: Keep or Mulligan?",
        Description = $"Keep {DefaultHandSize - decisions[pId].Taken} or go down to {DefaultHandSize - (decisions[pId].Taken + 1)}?",
        Actions = new[]
        {
          new GameAction("Keep", () => { decisions[pId].Keep = true; }),
          new GameAction("Mulligan", () => { decisions[pId].Taken++; }),
        }
      });

    foreach (var choice in choices)
    {
      yield return choice;
    }
  }

  public IEnumerable<GameEvent> PutCardsBack()
  {
    var players = decisions
      .Where(d => d.Value.Keep && d.Value.Taken > 0)
      .ToDictionary(d => d.Key, d => d.Value);

    foreach (var playerId in players.Keys)
    {
      var decision = players[playerId];
      var handId = (Zones.Hand, playerId);
      var libraryId = (Zones.Library, playerId);
      var playerName = game.GetPlayerName(playerId);
      for (int i = 0; i < decision.Taken; i++)
      {
        var remaining = decision.Taken - decision.PutBack.Count;
        yield return new GameEvent
        {
          Name = $"{playerName}: Pick a card to put back",
          Description = $"{remaining} remaining",
          Actions = game.Zones[handId]
            .Except(decisions[playerId].PutBack)
            .Select(c => new GameAction(c.ToString(), () => PutBack(playerId, c)))
            .ToArray()
        };
      }

      var hand = game.Zones[handId];
      var putBack = decisions[playerId].PutBack;
      var numPutBack = putBack.Count;
      var library = game.Zones[libraryId];
      game.UpdateZone(handId, hand.Except(putBack));
      game.UpdateZone(libraryId, putBack.Concat(library));
      yield return new GameEvent
      {
        Name = $"{playerName} put back {numPutBack}",
        Description = $@"{putBack.Aggregate("", (agg, cur) =>
        string.IsNullOrEmpty(agg)
          ? $" - {cur}"
          : $"{agg}\n - {cur}"
      )}" + Environment.NewLine,
      };
    }
  }

  void PutBack(string playerId, Card card) =>
    decisions[playerId].PutBack.Add(card);
}