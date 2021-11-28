using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace truename.Tests.MulliganSystemTests;

public class PutCardsBack : MulliganSystemTest
{
  public PutCardsBack(ITestOutputHelper output) : base(output)
  {
  }

  [Fact]
  public void Returns_no_events_if_player_is_done()
  {
    var playerId = game.ActivePlayerId;
    var otherPlayerId = game.TurnOrder.After(playerId);
    // this player is done deciding AND putting cards back
    // they should not get any events
    mulliganSystem.Decisions[playerId] = new MulliganDecision
    {
      Done = true,
      Keep = true,
      Taken = 0,
    };
    // this player has decided to keep but has not yet put
    // back their cards, they SHOULD get events
    mulliganSystem.Decisions[otherPlayerId] = new MulliganDecision
    {
      Keep = true,
      Taken = 1,
    };


    var playerIds = mulliganSystem
      .PutCardsBack()
      .Select(p => p.PlayerId)
      .ToArray();

    Assert.NotEmpty(playerIds);
    Assert.DoesNotContain(playerId, playerIds);
    Assert.Contains(otherPlayerId, playerIds);
  }
}