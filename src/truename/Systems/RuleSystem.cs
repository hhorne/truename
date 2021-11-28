using truename.Effects;
using truename.Effects.Predefined;

namespace truename.Systems;

public class RuleSystem
{
  private readonly Game game;
  private readonly HandSystem handSystem;
  private readonly LibrarySystem librarySystem;
  private readonly MulliganSystem mulliganSystem;
  private readonly TurnSystem turnSystem;
  private readonly Dictionary<string, IEnumerable<Func<GameEvent, GameEvent>>> TurnBasedActions = new();

  public RuleSystem(Game game)
  {
    this.game = game;
    handSystem = new HandSystem(game);
    librarySystem = new LibrarySystem(game);
    mulliganSystem = new MulliganSystem(game);
    turnSystem = new TurnSystem(game);
    TurnBasedActions = new()
    {
      [Turn.Untap] = new[]
      {
        (GameEvent @event) => new GameEvent(" - Phasing"),
        (GameEvent @event) => new GameEvent(" - Day/Night"),
        (GameEvent @event) => new GameEvent(" - Untap Permanents"),
      },
      [Turn.Upkeep] = new[]
      {
        (GameEvent @event) => PriorityGoesTo(@event.PlayerId)
      },
      [Turn.Draw] = new[]
      {
        (GameEvent @event) => DrawFromLibrary(game.ActivePlayerId),
        (GameEvent @event) => PriorityGoesTo(game.ActivePlayerId)
      },
      [Turn.PreCombatMain] = new[]
      {
        (GameEvent @event) => PriorityGoesTo(game.ActivePlayerId),
      },
      [Turn.BeginCombat] = new[]
      {
        (GameEvent @event) => PriorityGoesTo(game.ActivePlayerId),
      },
      [Turn.DeclareAttackers] = new[]
      {
        (GameEvent @event) => PriorityGoesTo(game.ActivePlayerId),
      },
      [Turn.DeclareBlockers] = new[]
      {
        (GameEvent @event) => PriorityGoesTo(game.ActivePlayerId),
      },
      [Turn.CombatDamage] = new[]
      {
        (GameEvent @event) => PriorityGoesTo(game.ActivePlayerId),
      },
      [Turn.EndCombat] = new[]
      {
        (GameEvent @event) => PriorityGoesTo(game.ActivePlayerId),
      },
      [Turn.PostCombatMain] = new[]
      {
        (GameEvent @event) => PriorityGoesTo(game.ActivePlayerId),
      },
      [Turn.EndStep] = new[]
      {
        (GameEvent @event) => PriorityGoesTo(game.ActivePlayerId),
      },
    };
  }

  public IEnumerable<GameEvent> PlayGame()
  {
    foreach (var @event in GameLoop())
      yield return LoggedEvent(@event);
  }

  public GameEvent LoggedEvent(GameEvent @event) => game.Log(@event);

  public IEnumerable<GameEvent> GameLoop()
  {
    foreach (var @event in DetermineTurnOrder())
      yield return @event;

    foreach (var @event in DrawOpeningHands())
      yield return @event;

    foreach (var @event in TakeTurns())
      yield return @event;
  }

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
      Choices = new[]
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
    for (int i = 0; i < DefaultHandSize; i++)
    {
      yield return DrawFromLibrary(playerId);
    }
  }

  GameEvent DrawFromLibrary(string playerId)
  {
    var playerName = game.GetPlayerName(playerId);
    var cards = librarySystem.TakeTop(playerId);
    handSystem.Draw(playerId, cards);
    return new GameEvent($" - {cards.First()}");
  }

  public IEnumerable<GameEvent> TakeTurns()
  {
    // this isn't event-sourced, maybe it doesn't need to be?
    game.ContinuousEffects.Add(new SkipFirstDraw());

    // for console test harness reasons
    var arbitraryConditionToEndOn = () => game.Turns[game.ActivePlayerId] < 4;
    while (game.TurnOrder.Count > 1 && arbitraryConditionToEndOn())
    {
      var turn = turnSystem.TakeTurn();
      foreach (var @event in turn)
      {
        var replacement = CheckForReplacement(@event);
        var result = replacement ?? @event;
        yield return result;

        if (TurnBasedActions.ContainsKey(result.Type))
        {
          foreach (var handler in TurnBasedActions[result.Type])
            yield return handler(result);
        }
      }
    }
  }

  public GameEvent? CheckForReplacement(GameEvent @event)
  {
    var replacement = game.ContinuousEffects
      .OfType<ReplacementEffect>()
      .FirstOrDefault(x => x.AppliesTo(game, @event));

    if (replacement is not null && replacement.IsExpired(game, @event))
      game.ContinuousEffects.Remove(replacement);

    return replacement?.Event(game, @event);
  }

  public GameEvent PriorityGoesTo(string playerId)
  {
    game.GivePriorityTo(playerId);
    return new GameEvent($"{game.ActivePlayer.Name} gains priority");
  }
}