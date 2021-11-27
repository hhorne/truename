using System.Linq;
using Xunit;

namespace truename.Systems;

public class MulliganSystemTests
{
  [Fact]
  public void DeclareMulligans_goes_in_turn_order()
  {
    var players = new[] {
      new Player("tron_travolta", TestData.GrixisDeck),
      new Player("typedef", TestData.ReanimatorDeck)
    };

    var game = new Game(players);
    var mulliganSystem = new MulliganSystem(game);
    game.SetTurnOrder(players.Select(p => p.Id));
    mulliganSystem.Init();

    var turnOrder = game.TurnOrder;
    var playerIds = mulliganSystem
      .DeclareMulligans()
      .Select(p => p.PlayerId)
      .ToList();

    var ordersMatch = Enumerable.SequenceEqual(playerIds, turnOrder);
    Assert.True(ordersMatch);
  }
}