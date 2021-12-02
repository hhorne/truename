using truename.Effects;
using truename.Effects.Predefined;

namespace truename.Systems;

delegate EventDescription TurnBasedAction(Game game);
delegate IEnumerable<EventDescription> GameEvent(Game game);

public class RuleSystem
{
  private readonly Game game;
  private readonly HandSystem handSystem;
  private readonly LibrarySystem librarySystem;
  private readonly MulliganSystem mulliganSystem;
  private readonly TimingSystem timingSystem;
  private readonly TurnSystem turnSystem;
  private readonly Dictionary<string, IEnumerable<GameEvent>> TurnBasedActions = new();

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
      [Turn.Steps.Untap] = new GameEvent[]
      {
        g => Phasing(),
        g => DayNight(),
        g => UntapPermanents(),
      },
      [Turn.Steps.Draw] = new GameEvent[]
      {
        g => new [] { DrawFromLibrary(g.ActivePlayerId) },
      },
      [Turn.Phases.PreCombatMain] = new GameEvent[]
      {
        g => Sagas(),
      },
      [Turn.Steps.DeclareAttackers] = new GameEvent[]
      {
        g => DeclareAttackers(),
      },
      [Turn.Steps.DeclareBlockers] = new GameEvent[]
      {
        g => DeclareBlockers(),
      },
      [Turn.Steps.CombatDamage] = new GameEvent[]
      {
        g => CombatDamage(),
      },
      [Turn.Steps.Cleanup] = new GameEvent[]
      {
        g => DiscardToHandSize(),
        g => RemoveDamage(),
        g => EndCleanup(),
      },
    };
  }

  public IEnumerable<EventDescription> PlayGame()
  {
    foreach (var @event in GameLoop())
      yield return LoggedEvent(@event);
  }

  public EventDescription LoggedEvent(EventDescription @event) => game.Log(@event);

  public IEnumerable<EventDescription> GameLoop()
  {
    yield return DetermineTurnOrder();

    var playerName = game.ActivePlayer.Name;
    yield return new EventDescription($"{playerName} on the play");

    foreach (var @event in DrawOpeningHands())
      yield return @event;

    foreach (var @event in TakeTurns())
    {
      yield return @event;
      // exchange priority
    }
  }

  EventDescription DetermineTurnOrder()
  {
    var turnOrder = game
      .Players
      .Keys
      .ToArray()
      .Shuffle();

    var winnerId = turnOrder.First();
    var player = game.Players[winnerId];
    return new Decision
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
  }

  IEnumerable<EventDescription> DrawOpeningHands()
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
          handSystem.Remove(playerId, hand);
          librarySystem.PutOnBottom(playerId, hand);
        }

        librarySystem.Shuffle(playerId);
        var playerName = game.GetPlayerName(playerId);
        yield return new EventDescription
        {
          Name = $"{playerName} puts hand on bottom of Library and Shuffles it away.",
        };
      }

      foreach (var playerId in stillDeciding)
        foreach (var @event in DrawHand(playerId))
          yield return @event;

      foreach (var @event in mulliganSystem.DeclareMulligans())
        yield return @event;
    }
    while (mulliganSystem.StillDeciding.Any());
  }

  IEnumerable<EventDescription> DrawHand(string playerId)
  {
    const int DefaultHandSize = 7;
    var playerName = game.GetPlayerName(playerId);
    yield return new EventDescription($"{playerName} draws hand:");
    for (int i = 0; i < DefaultHandSize; i++)
    {
      yield return DrawFromLibrary(playerId);
    }
  }

  EventDescription DrawFromLibrary(string playerId)
  {
    var cards = librarySystem.TakeTop(playerId);
    handSystem.Draw(playerId, cards);
    return new EventDescription($"{game.GetPlayerName(playerId)} drew {cards.First()}");
  }

  public IEnumerable<EventDescription> TakeTurns()
  {
    // this isn't event-sourced, maybe it doesn't need to be?
    game.ContinuousEffects.Add(new SkipFirstDraw());

    // for console test harness reasons
    var arbitraryConditionToEndOn = () => game.Turns[game.ActivePlayerId] < 4;
    while (game.TurnOrder.Count > 1 && arbitraryConditionToEndOn())
    {
      foreach (var @event in turnSystem.TakeTurn())
      {
        // ReplacementEffects that skip (or add?) Turn Phases and/or Steps
        var result = FindReplacementEffect(@event) ?? @event;

        // Look for Turn-Based Actions
        if (TurnBasedActions.TryGetValue(@event.Type, out var actions))
          foreach (var action in actions)
            foreach (var step in action(game))
              yield return step;

        // check for beginning of step/phase triggers
        yield return result;
        // check for end of step/phase triggers
      }
    }
  }

  public EventDescription? FindReplacementEffect(EventDescription incomingEvent)
  {
    var foundEffect = game.ContinuousEffects
      .OfType<ReplacementEffect>()
      .FirstOrDefault(x => x.AppliesTo(game, incomingEvent));

    if (foundEffect != null && foundEffect.IsExpired(game, incomingEvent))
      game.ContinuousEffects.Remove(foundEffect);

    return foundEffect?.CreateReplacement(game, incomingEvent);
  }

  public EventDescription PriorityGoesTo(string playerId)
  {
    game.GivePriorityTo(playerId);
    return new EventDescription($"{game.ActivePlayer.Name} gains priority");
  }

  public IEnumerable<EventDescription> Phasing()
  {
    yield return new EventDescription("502.1. Phasing")
    {
      Description = "All phased-in permanents with phasing that the active player controls phase out, and all phased-out permanents that the active player controlled when they phased out phase in. This all happens simultaneously."
    };
  }

  public IEnumerable<EventDescription> DayNight()
  {
    yield return new EventDescription("502.2. Day/Night")
    {
      Description = "If it’s day and the previous turn’s active player didn’t cast any spells during that turn, it becomes night. If it’s night and the previous turn’s active player cast two or more spells during that turn, it becomes day. If it’s neither day nor night, this check doesn’t happen and it remains neither.",
    };
  }

  public IEnumerable<EventDescription> UntapPermanents()
  {
    yield return new EventDescription("502.3. Untap Permanents")
    {
      Description = "The active player determines which permanents they control will untap. Then they untap them all simultaneously. This turn-based action doesn’t use the stack. All of a player’s permanents untap unless an effect prevents one or more of a player’s permanents from untapping.",
    };
  }

  public IEnumerable<EventDescription> Sagas()
  {
    yield return new EventDescription("505.4. Sagas")
    {
      Description = "If the active player controls one or more Saga enchantments and it’s the active player’s precombat main phase, the active player puts a lore counter on each Saga they control.",
    };
  }

  public IEnumerable<EventDescription> DeclareAttackers()
  {
    yield return new EventDescription("508.1. First, the active player declares attackers.")
    {
      Description = "There are A LOT of parts to this so look at the Comprehensive Rules. This might inform/dictate further larger design.",
    };
  }

  public IEnumerable<EventDescription> DeclareBlockers()
  {
    yield return new EventDescription("509.1. First, the defending player declares blockers.")
    {
      Description = "There are A LOT of parts to this so look at the Comprehensive Rules. This might inform/dictate further larger design.",
    };
  }

  public IEnumerable<EventDescription> CombatDamage()
  {
    yield return new EventDescription()
    {
      Name = "510.1. First, the active player announces how each attacking creature assigns its combat damage",
      Description = "Then the defending player announces how each blocking creature assigns its combat damage."
    };
  }

  public IEnumerable<EventDescription> DiscardToHandSize()
  {
    yield return new EventDescription("514.1. active player’s hand contains more cards than their maximum hand size (normally seven), they discard enough cards to reduce their hand size to that number.");
  }

  public IEnumerable<EventDescription> RemoveDamage()
  {
    yield return new EventDescription("514.2. Damage marked on permanents is removed, all \"until end of turn\" and \"this turn\" effects end.");
  }

  public IEnumerable<EventDescription> EndCleanup()
  {
    yield return new EventDescription("514.3. Normally, no player receives priority during the cleanup step, so no spells can be cast and no abilities can be activated. However, this rule is subject to the following exception: ")
    {
      Description = $"514.3a At this point, the game checks to see if any state-based actions would be performed and/or any triggered abilities are waiting to be put onto the stack (including those that trigger “at the beginning of the next cleanup step”). If so, those state-based actions are performed, then those triggered abilities are put on the stack, then the active player gets priority. Players may cast spells and activate abilities. Once the stack is empty and all players pass in succession, another cleanup step begins."
    };
  }
}
