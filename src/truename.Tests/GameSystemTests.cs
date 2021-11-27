using System;
using System.Collections.Generic;
using Xunit;
using truename.Systems;

namespace truename.Tests;

public class GameSystemTests
{
  [Fact]
  public void Test1()
  {
    var game = new Game(new[]{
      new Player(
        "tron_travolta",
        TestData.GrixisDeck
      ),
      new Player(
        "typedef",
        TestData.ReanimatorDeck
      )
    });

    game.SetTurnOrder(new[]
    {
      "tron_travolta",
      "typedef"
    });

    var @event = new GameEvent
    {
      PlayerId = game.ActivePlayerId,
      Name = $"{game.ActivePlayer.Name}'s Draw Step",
      Type = "Turn/Step/Draw",
      Actions = new[]
      {
        new GameAction("Draw", () => {}),
      },
    };
  }
}

public class ContinuousEffect
{
  Predicate<Game> condition = g => false;
  Predicate<Game> expired = g => false;
  public Action Effect { get; set; } = () => { };

  public bool Applies(Game g) => condition(g);
  public bool Expired(Game g) => expired(g);
}

public class ReplacementEffect : ContinuousEffect
{

}