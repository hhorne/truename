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
    var game = new Game();
    var p1 = new Player(
      "tron_travolta",
      TestData.GrixisDeck
    );

    var p2 = new Player(
      "typedef",
      TestData.ReanimatorDeck
    );

    game.Players = new Dictionary<Guid, Player>
      {
          { p1.Id, p1 },
          { p2.Id, p2 }
      };

    var ruleSystem = new RuleSystem(game);
    ruleSystem.PlayGame();
  }
}