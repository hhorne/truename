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
        (GameEvent @event) => new GameEvent("502.1. Phasing")
        {
          Description = "All phased-in permanents with phasing that the active player controls phase out, and all phased-out permanents that the active player controlled when they phased out phase in. This all happens simultaneously."
        },
        (GameEvent @event) => new GameEvent("502.2. Day/Night")
        {
          Description = "If it’s day and the previous turn’s active player didn’t cast any spells during that turn, it becomes night. If it’s night and the previous turn’s active player cast two or more spells during that turn, it becomes day. If it’s neither day nor night, this check doesn’t happen and it remains neither.",
        },
        (GameEvent @event) => new GameEvent("502.3. Untap Permanents")
        {
          Description = "The active player determines which permanents they control will untap. Then they untap them all simultaneously. This turn-based action doesn’t use the stack. All of a player’s permanents untap unless an effect prevents one or more of a player’s permanents from untapping.",
        },
      },
      [Turn.Draw] = new[]
      {
        (GameEvent @event) => DrawFromLibrary(game.ActivePlayerId),
      },
      [Turn.PreCombatMain] = new[]
      {
        (GameEvent @event) => new GameEvent("505.4. Sagas")
        {
          Description = "If the active player controls one or more Saga enchantments and it’s the active player’s precombat main phase, the active player puts a lore counter on each Saga they control.",
        },
      },
      [Turn.DeclareAttackers] = new[]
      {
        (GameEvent @event) => new GameEvent("508.1. First, the active player declares attackers.")
        {
          Description = "There are A LOT of parts to this so look at the Comprehensive Rules. This might inform/dictate further larger design.",
        },
      },
      [Turn.DeclareBlockers] = new[]
      {
        (GameEvent @event) => new GameEvent("509.1. First, the defending player declares blockers.")
        {
          Description = "There are A LOT of parts to this so look at the Comprehensive Rules. This might inform/dictate further larger design.",
        },
      },
      [Turn.CombatDamage] = new[]
      {
        (GameEvent @event) => new GameEvent()
        {
          Name = "510.1. First, the active player announces how each attacking creature assigns its combat damage",
          Description = "Then the defending player announces how each blocking creature assigns its combat damage."
        },
      },
      [Turn.Cleanup] = new[]
      {
        (GameEvent @event) => new GameEvent("514.1. active player’s hand contains more cards than their maximum hand size (normally seven), they discard enough cards to reduce their hand size to that number."),
        (GameEvent @event) => new GameEvent("514.2. Damage marked on permanents is removed, all \"until end of turn\" and \"this turn\" effects end."),
        (GameEvent @event) => new GameEvent("514.3. Normally, no player receives priority during the cleanup step, so no spells can be cast and no abilities can be activated. However, this rule is subject to the following exception: ")
        {
          Description = $"514.3a At this point, the game checks to see if any state-based actions would be performed and/or any triggered abilities are waiting to be put onto the stack (including those that trigger “at the beginning of the next cleanup step”). If so, those state-based actions are performed, then those triggered abilities are put on the stack, then the active player gets priority. Players may cast spells and activate abilities. Once the stack is empty and all players pass in succession, another cleanup step begins."
        },
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
    return new GameEvent($" - [bold cornflowerblue]{cards.First()}[/]");
  }

  public IEnumerable<GameEvent> TakeTurns()
  {
    // this isn't event-sourced, maybe it doesn't need to be?
    game.ContinuousEffects.Add(new SkipFirstDraw());

    // for console test harness reasons
    var arbitraryConditionToEndOn = () => game.Turns[game.ActivePlayerId] < 4;
    while (game.TurnOrder.Count > 1 && arbitraryConditionToEndOn())
    {
      foreach (var @event in turnSystem.TakeTurn())
      {
        TryGetReplacementEffect(@event, out var replacement);

        var result = replacement ?? @event;
        if (result.Type.StartsWith(Turn.BaseEventKey))
          game.UpdateTurnStep(result.Type);
        
        yield return result;

        if (TurnBasedActions.TryGetValue(result.Type, out var actions))
          foreach (var handler in actions)
            yield return handler(result);

        var priorityExchange = game.TurnStep switch
        {
          string turnStep when turnStep.Contains("fart") => Enumerable.Empty<GameEvent>(),
        };
        // handle priority
        // foreach (var @event in timingSystem.ExchangePriority())
        //   yield @event;
      }
    }
  }

  public bool TryGetReplacementEffect(GameEvent @event, out GameEvent? replacement)
  {
    // todo
    // currently only grabbing the first. eventually have to open
    // the "choose/order multiples" can of worms
    replacement = null;
    var applicableEffect = game.ContinuousEffects
      .OfType<ReplacementEffect>()
      .FirstOrDefault(x => x.AppliesTo(game, @event));

    if (applicableEffect is null)
      return false;
    
    if (applicableEffect.IsExpired(game, @event))
      game.ContinuousEffects.Remove(applicableEffect);

    replacement = applicableEffect.CreateReplacement(game, @event);
    return true;
  }

  public GameEvent PriorityGoesTo(string playerId)
  {
    game.GivePriorityTo(playerId);
    return new GameEvent($"{game.ActivePlayer.Name} gains priority");
  }
}
