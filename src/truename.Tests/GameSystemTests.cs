using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using truename.Effects;
using truename.Effects.Predefined;

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
    };

    var skipDraw = new SkipFirstDraw();
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

    var skipDraw = new SkipFirstDraw();

    game.ContinuousEffects.Add(skipDraw);
    Assert.True(game.ContinuousEffects.OfType<ReplacementEffect>().Any());
  }
}