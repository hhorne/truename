using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using truename.Effects;

namespace truename.Tests;

public class GameSystemTests
{
  [Fact]
  public void Test1()
  {
    var players = new[] {
      new Player("tron_travolta", TestData.GrixisDeck),
      new Player("typedef", TestData.ReanimatorDeck)
    };

    var game = new Game(players);
    game.SetTurnOrder(players.Select(p => p.Id));

    var drawEventId = "Turn/Step/Draw";
    var @event = new GameEvent
    {
      PlayerId = game.ActivePlayerId,
      Name = "Draw Step",
      Description = $"{game.ActivePlayer.Name}'s Draw Step",
      Type = drawEventId,
      Actions = new[]
      {
        new GameAction("Draw", () => {}),
      },
    };

    GameCondition isFirstDraw = (game, @event) =>
    {
      if (@event.Type == drawEventId)
      {
        var firstPlayer = game.TurnOrder.First();
        var playerId = @event.PlayerId;
        var turnNumber = game.Turns[playerId];
        return turnNumber == 1 && firstPlayer == playerId;
      }

      return false;
    };

    var events = new[]
    {
      new GameEvent
      {
        Name = "Skip Draw",
        Type = $"Skip/{drawEventId}"
      }
    };

    var skipDraw = new ReplacementEffect(isFirstDraw, events);
    Assert.True(skipDraw.AppliesTo(game, @event));
  }

  [Fact]
  public void Test2()
  {
    var players = new[] {
      new Player("tron_travolta", TestData.GrixisDeck),
      new Player("typedef", TestData.ReanimatorDeck)
    };

    var game = new Game(players);
    game.SetTurnOrder(players.Select(p => p.Id));

    var skipDraw = new SkipDraw((game, @event) =>
    {
      if (@event.Type == "Turn/Step/Draw")
      {
        var firstPlayer = game.TurnOrder.First();
        var playerId = @event.PlayerId;
        var turnNumber = game.Turns[playerId];
        return turnNumber == 1 && firstPlayer == playerId;
      }

      return false;
    });

    game.ContinuousEffects.Add(skipDraw);
    Assert.True(game.ContinuousEffects.OfType<ReplacementEffect>().Any());
  }
}