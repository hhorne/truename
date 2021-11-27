using truename.Effects;

namespace truename.Systems;

public class RuleSystem
{
  private readonly Game game;
  private readonly HandSystem handSystem;
  private readonly LibrarySystem librarySystem;
  private readonly MulliganSystem mulliganSystem;
  private readonly TurnSystem turnSystem;

  public RuleSystem(Game game)
  {
    this.game = game;
    handSystem = new HandSystem(game);
    librarySystem = new LibrarySystem(game);
    mulliganSystem = new MulliganSystem(game);
    turnSystem = new TurnSystem(game);
  }

  public IEnumerable<GameEvent> PlayGame()
  {
    foreach (var @event in GameLoop())
    {
      yield return @event;
    }
  }

  public IEnumerable<GameEvent> GameLoop()
  {
    foreach (var @event in DetermineTurnOrder())
      yield return LoggedEvent(@event);

    foreach (var @event in DrawOpeningHands())
      yield return LoggedEvent(@event);

    foreach (var @event in TakeTurns())
    {
      // game.ContinuousEffects.OfType<ReplacementEffect>()
      // effect replacement?
      if (@event.Type == "Untap")
        yield return LoggedEvent(new GameEvent("Skipped Untap"));
      else
        yield return LoggedEvent(@event);
    }
  }

  public GameEvent LoggedEvent(GameEvent @event) => game.Log(@event);

  IEnumerable<GameEvent> DetermineTurnOrder()
  {
    var turnOrder = game
      .Players
      .Keys
      .ToArray()
      .Shuffle();

    var winnerId = turnOrder.First();
    var player = game.Players[winnerId];
    yield return new GameEvent
    {
      Name = $"{player.Name} won the die roll",
      Description = "Go First?",
      Actions = new[]
      {
        new GameAction("Play", () => game.SetTurnOrder(turnOrder)),
        new GameAction("Draw", () => game.SetTurnOrder(turnOrder.Reverse())),
      }
    };

    yield return new GameEvent($"{game.ActivePlayer.Name} on the play");
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
        foreach (var @event in DrawHand(playerId))
          yield return @event;

      var mulliganChoices = mulliganSystem.DeclareMulligans();
      foreach (var @event in mulliganChoices)
        yield return @event;
    }
    while (mulliganSystem.StillDeciding.Any());
  }

  IEnumerable<GameEvent> DrawHand(string playerId)
  {
    var playerName = game.GetPlayerName(playerId);
    yield return new GameEvent($"{playerName} draws hand:");
    const int DefaultHandSize = 7;
    var cardsDrawn = new List<Card>();
    for (int i = 0; i < DefaultHandSize; i++)
    {
      var cards = librarySystem.TakeTop(playerId);
      handSystem.Draw(playerId, cards);
      cardsDrawn.AddRange(cards);
      yield return new GameEvent($" - {cards.First()}");
    }
  }

  public IEnumerable<GameEvent> TakeTurns()
  {
    // this isn't event-sourced, maybe it doesn't need to be?
    game.ContinuousEffects.Add(new SkipDraw((game, @event) =>
    {
      if (@event.Type == "Turn/Step/Draw")
      {
        var firstPlayer = game.TurnOrder.First();
        var playerId = @event.PlayerId;
        var turnNumber = game.Turns[playerId];
        return turnNumber == 1 && firstPlayer == playerId;
      }

      return false;
    }));

    while (game.TurnOrder.Count > 1 && game.Turns[game.ActivePlayerId] < 4)
    {
      var turn = turnSystem.TakeTurn();
      foreach (var @event in turn)
      {
        var replacement = game
          .ContinuousEffects
          .OfType<ReplacementEffect>()
          .FirstOrDefault(x => x.AppliesTo(game, @event));

        if (replacement is null)
        {
          yield return @event;
        }
        else
        {
          foreach (var effect in replacement.Events)
            yield return effect;
        }
      }
    }
  }
}