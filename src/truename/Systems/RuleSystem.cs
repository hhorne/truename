using truename.Effects;
using truename.Effects.Predefined;

namespace truename.Systems;

public class RuleSystem
{
  private readonly Game game;
  private readonly HandSystem handSystem;
  private readonly LibrarySystem librarySystem;
  private readonly MulliganSystem mulliganSystem;
  private readonly TimingSystem timingSystem;
  private readonly TurnSystem turnSystem;
  private readonly Dictionary<string, IEnumerable<Func<GameEvent, IEnumerable<GameEvent>>>> TurnBasedActions = new();

  public RuleSystem(Game game)
  {
    this.game = game;
    handSystem = new HandSystem(game);
    librarySystem = new LibrarySystem(game);
    mulliganSystem = new MulliganSystem(game);
    timingSystem = new TimingSystem(game);
    turnSystem = new TurnSystem(game);

    TurnBasedActions = new()
    {
      [Turn.Steps.Untap] = new[]
      {
        (GameEvent @event) => Phasing(),
        (GameEvent @event) => DayNight(),
        (GameEvent @event) => UntapPermanents(),
      },
      [Turn.Steps.Draw] = new[]
      {
        (GameEvent @event) => DrawFromLibrary(game.ActivePlayerId),
      },
      [Turn.Phases.PreCombatMain] = new[]
      {
        (GameEvent @event) => Sagas(),
      },
      [Turn.Steps.DeclareAttackers] = new[]
      {
        (GameEvent @event) => DeclareAttackers(),
      },
      [Turn.Steps.DeclareBlockers] = new[]
      {
        (GameEvent @event) => DeclareBlockers(),
      },
      [Turn.Steps.CombatDamage] = new[]
      {
        (GameEvent @event) => CombatDamage(),
      },
      [Turn.Steps.Cleanup] = new[]
      {
        (GameEvent @event) => DiscardToHandSize(),
        (GameEvent @event) => RemoveDamage(),
        (GameEvent @event) => EndCleanup(),
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
        // not according to the Comprehensive Rules but were just 1v1 right now
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
          yield return handSystem.Take(playerId, hand, out var cards);
          yield return librarySystem.PutOnBottom(playerId, cards);
        }

        yield return librarySystem.Shuffle(playerId);
      }

      foreach (var playerId in stillDeciding)
        foreach (var @event in DrawHand(playerId))
          yield return @event;

      foreach (var @event in mulliganSystem.DeclareMulligans())
        yield return @event;
    }
    while (mulliganSystem.StillDeciding.Any());
  }

  IEnumerable<GameEvent> DrawHand(string playerId)
  {
    const int DefaultHandSize = 7;
    var playerName = game.GetPlayerName(playerId);
    yield return new GameEvent($"{playerName} draws hand:");
    for (int i = 0; i < DefaultHandSize; i++)
    {
      foreach(var @event in DrawFromLibrary(playerId))
        yield return @event;
    }
  }

  IEnumerable<GameEvent> DrawFromLibrary(string playerId)
  {
    yield return librarySystem.TakeTop(playerId, out var cards);
    yield return handSystem.Draw(playerId, cards);
  }

  public IEnumerable<GameEvent> TakeTurns()
  {
    // this isn't event-sourced, maybe it doesn't need to be?
    game.ContinuousEffects.Add(new SkipFirstDraw());

    // for console test harness reasons
    var arbitraryConditionToEndOn = () => game.Turns[game.ActivePlayerId] < 4;
    while (game.TurnOrder.Count > 1 && arbitraryConditionToEndOn())
    {
      foreach (var turnPart in turnSystem.TakeTurn())
      {
        // a lot of this should probably be elevated to the foreach
        // loop in order to be applied as broadly as possible
        TryGetReplacementEffect(turnPart, out var replacement);

        var result = replacement ?? turnPart;
        if (result.Type.StartsWith(Turn.Steps.BaseKey))
          game.UpdateTurnStep(result.Type);
        
        yield return result;

        if (TurnBasedActions.TryGetValue(result.Type, out var actions))
        {
          foreach (var action in actions)
            foreach (var subAction in action(result))
              yield return subAction;
        }

        var priorityExchange = game.TurnStep switch
        {
          string turnStep when Turn.PartsWithPriority.Contains(turnStep) => Enumerable.Empty<GameEvent>(),
          _ => timingSystem.ExchangePriority()
        };

        // handle priority
        foreach (var action in priorityExchange)
          yield return action;
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

  public IEnumerable<GameEvent> Phasing()
  {
    yield return new GameEvent("502.1. Phasing")
    {
      Description = "All phased-in permanents with phasing that the active player controls phase out, and all phased-out permanents that the active player controlled when they phased out phase in. This all happens simultaneously."
    };
  }

  public IEnumerable<GameEvent> DayNight()
  {
    yield return new GameEvent("502.2. Day/Night")
    {
      Description = "If it’s day and the previous turn’s active player didn’t cast any spells during that turn, it becomes night. If it’s night and the previous turn’s active player cast two or more spells during that turn, it becomes day. If it’s neither day nor night, this check doesn’t happen and it remains neither.",
    };
  }

  public IEnumerable<GameEvent> UntapPermanents()
  {
    yield return new GameEvent("502.3. Untap Permanents")
    {
      Description = "The active player determines which permanents they control will untap. Then they untap them all simultaneously. This turn-based action doesn’t use the stack. All of a player’s permanents untap unless an effect prevents one or more of a player’s permanents from untapping.",
    };
  }

  public IEnumerable<GameEvent> Sagas()
  {
    yield return new GameEvent("505.4. Sagas")
    {
      Description = "If the active player controls one or more Saga enchantments and it’s the active player’s precombat main phase, the active player puts a lore counter on each Saga they control.",
    };
  }

  public IEnumerable<GameEvent> DeclareAttackers()
  {
    yield return new GameEvent("508.1. First, the active player declares attackers.")
    {
      Description = "There are A LOT of parts to this so look at the Comprehensive Rules. This might inform/dictate further larger design.",
    };
  }

  public IEnumerable<GameEvent> DeclareBlockers()
  {
    yield return new GameEvent("509.1. First, the defending player declares blockers.")
    {
      Description = "There are A LOT of parts to this so look at the Comprehensive Rules. This might inform/dictate further larger design.",
    };
  }

  public IEnumerable<GameEvent> CombatDamage()
  {
    yield return new GameEvent()
    {
      Name = "510.1. First, the active player announces how each attacking creature assigns its combat damage",
      Description = "Then the defending player announces how each blocking creature assigns its combat damage."
    };
  }

  public IEnumerable<GameEvent> DiscardToHandSize()
  {
    yield return new GameEvent("514.1. active player’s hand contains more cards than their maximum hand size (normally seven), they discard enough cards to reduce their hand size to that number.");
  }

  public IEnumerable<GameEvent> RemoveDamage()
  {
    yield return new GameEvent("514.2. Damage marked on permanents is removed, all \"until end of turn\" and \"this turn\" effects end.");
  }

  public IEnumerable<GameEvent> EndCleanup()
  {
    yield return new GameEvent("514.3. Normally, no player receives priority during the cleanup step, so no spells can be cast and no abilities can be activated. However, this rule is subject to the following exception: ")
    {
      Description = $"514.3a At this point, the game checks to see if any state-based actions would be performed and/or any triggered abilities are waiting to be put onto the stack (including those that trigger “at the beginning of the next cleanup step”). If so, those state-based actions are performed, then those triggered abilities are put on the stack, then the active player gets priority. Players may cast spells and activate abilities. Once the stack is empty and all players pass in succession, another cleanup step begins."
    };
  }
}
