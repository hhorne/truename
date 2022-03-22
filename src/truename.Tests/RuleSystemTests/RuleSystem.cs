using System;
using System.Collections.Generic;
using System.Linq;
using truename.Events;

namespace truename.Tests.RuleSystemTests;

public class RuleSystem
{
  private readonly Game game;
  public RuleSystem(Game game)
  {
    this.game = game;
  }

  public IEnumerable<IGameEvent> PlayGame()
  {
    foreach (var @event in DetermineTurnOrder())
      yield return @event;
  }

  public IEnumerable<IGameEvent> DetermineTurnOrder()
  {
    var turnOrder = game
      .Players
      .Keys
      .ToArray()
      .Shuffle();

    var winnerId = turnOrder.First();
    var player = game.Players[winnerId];
    SetTurnOrder? @event = null;
    yield return new Decision
    {
      Name = $"{player.Name} won the die roll",
      Description = "Go First?",
      Choices = new[]
      {
        new GameAction("Play", () => @event = new SetTurnOrder(winnerId, turnOrder)),
        new GameAction("Draw", () => @event = new SetTurnOrder(winnerId, turnOrder.Reverse())),
      }
    };

    if (@event is null)
    {
      throw new Exception("RuleSystem.DetermineTurnOrder cannot progress without a Choice being made.");
    }

    game.Apply(@event);
    yield return @event;
  }

  public IEnumerable<IGameEvent> DrawOpeningHands() => Enumerable.Empty<IGameEvent>();
}