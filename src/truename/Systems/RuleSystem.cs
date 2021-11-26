namespace truename.Systems;

public class RuleSystem
{
  private readonly Game game;
  private HandSystem handSystem;
  private LibrarySystem librarySystem;
  private MulliganSystem mulliganSystem;

  (Zones, Guid?) handFor(Guid playerId) => (Zones.Hand, playerId);

  public RuleSystem(Game game)
  {
    this.game = game;
    handSystem = new HandSystem(game);
    librarySystem = new LibrarySystem(game);
    mulliganSystem = new MulliganSystem(game);
  }

  public IEnumerable<GameEvent> PlayGame()
  {
    yield return DetermineTurnOrder();
    var playerId = game.TurnOrder.First();
    var playerName = game.GetPlayerName(playerId);
    yield return new GameEvent($"{playerName} on the play");

    foreach (var @event in DrawOpeningHands())
      yield return @event;

    // foreach (var @event in TakeTurns())
    //   yield return @event;
  }

  GameEvent DetermineTurnOrder()
  {
    var turnOrder = game
      .Players
      .Keys
      .ToArray()
      .Shuffle();

    var winnerId = turnOrder.First();
    var player = game.Players[winnerId];
    return new GameEvent
    {
      Name = $"{player.Name} won the die roll",
      Description = "Go First?",
      Options = new[]
      {
        new GameAction("Play", () => { game.SetTurnOrder(turnOrder); }),
        new GameAction("Draw", () => { game.SetTurnOrder(turnOrder.Reverse()); }),
      }
    };
  }

  IEnumerable<GameEvent> DrawOpeningHands()
  {
    mulliganSystem.Init();
    do
    {
      var stillDeciding = mulliganSystem.StillDeciding;
      foreach (var playerId in stillDeciding)
      {
        var hand = handSystem.HandFor(playerId);
        if (hand.Any())
        {
          var cards = handSystem.Take(playerId, hand);
          yield return librarySystem.PutOnBottom(playerId, cards);
        }

        yield return librarySystem.Shuffle(playerId);
      }

      foreach (var playerId in stillDeciding)
        yield return DrawHand(playerId);

      var mulliganChoices = mulliganSystem.DeclareMulligans();
      foreach (var @event in mulliganChoices)
        yield return @event;
    }
    while (mulliganSystem.StillDeciding.Any());
  }

  GameEvent DrawHand(Guid playerId)
  {
    const int DefaultHandSize = 7;
    var cardsDrawn = new List<string>();
    for (int i = 0; i < DefaultHandSize; i++)
    {
      var cards = librarySystem.TakeTop(playerId);
      handSystem.Draw(playerId, cards);
      cardsDrawn.AddRange(cards);
    }

    return new GameEvent
    {
      Name = $"{game.GetPlayerName(playerId)} draws...",
      Description = $@"{cardsDrawn.Aggregate("", (agg, cur) =>
        string.IsNullOrEmpty(agg)
          ? $" - {cur}"
          : $"{agg}\n - {cur}"
      )}" + Environment.NewLine,
    };
  }

  // public IEnumerable<GameEvent> TakeTurns()
  // {
  //   // seed turn queue with first turn
  //   do
  //   {
  //   } while (game.TurnOrder.Count > 1);
  //   return Enumerable.Empty<GameEvent>();
  // }
}