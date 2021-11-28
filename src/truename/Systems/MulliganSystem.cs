namespace truename.Systems;

public class MulliganSystem
{
  int DefaultHandSize = 7;
  private readonly Game game;
  public Dictionary<string, MulliganDecision> Decisions { get; set; } = new();

  public IEnumerable<string> StillDeciding => Decisions
    .Where(d => !d.Value.Keep)
    .Select(d => d.Key);

  public MulliganSystem(Game game)
  {
    this.game = game;
  }

  public void Init()
  {
    Decisions = game
      .TurnOrder
      .ToDictionary(
        p => p,
        p => new MulliganDecision()
      );
  }

  public IEnumerable<GameEvent> DeclareMulligans()
  {
    foreach (var k in KeepOrMull())
    {
      yield return k;

      foreach (var p in PutCardsBack())
        yield return p;
    }
  }

  public IEnumerable<GameEvent> KeepOrMull()
  {
    var choices = StillDeciding
      .Select(pId => new GameEvent
      {
        PlayerId = pId,
        Name = $"{game.GetPlayerName(pId)}: Keep or Mulligan?",
        Description = $"Keep {DefaultHandSize - Decisions[pId].Taken} or go down to {DefaultHandSize - (Decisions[pId].Taken + 1)}?",
        Choices = new[]
        {
          new GameAction("Keep", () => { Decisions[pId].Keep = true; }),
          new GameAction("Mulligan", () => { Decisions[pId].Taken++; }),
        }
      });

    foreach (var choice in choices)
      yield return choice;
  }

  public IEnumerable<GameEvent> PutCardsBack()
  {
    var players = Decisions
      .Where(d =>
        d.Value.Keep &&
        d.Value.Taken > 0 &&
        !d.Value.Done)
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
          PlayerId = playerId,
          Name = $"{playerName}: Pick a card to put back",
          Description = $"{remaining} remaining",
          Choices = game.Zones[handId]
            .Except(Decisions[playerId].PutBack)
            .Select(c => new GameAction(c.ToString(), () => { PutBack(playerId, c); }))
            .ToArray()
        };
      }

      Decisions[playerId].Done = true;
      var hand = game.Zones[handId];
      var putBack = Decisions[playerId].PutBack;
      var numPutBack = putBack.Count;
      var library = game.Zones[libraryId];
      game.UpdateZone(handId, hand.Except(putBack));
      game.UpdateZone(libraryId, putBack.Concat(library));

      if (numPutBack > 0)
      {
        var listOfCards = string.Join(
          Environment.NewLine,
          putBack.Select(c => $" - {c}")
        );

        yield return new GameEvent
        {
          PlayerId = playerId,
          Name = $"{playerName} put back {numPutBack}",
          Description = $"{listOfCards}{Environment.NewLine}",
        };
      }
    }
  }

  void PutBack(string playerId, Card card) =>
    Decisions[playerId].PutBack.Add(card);
}