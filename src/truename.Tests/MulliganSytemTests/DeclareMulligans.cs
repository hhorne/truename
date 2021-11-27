using System.Linq;
using truename.Systems;
using Xunit;
using Xunit.Abstractions;

namespace truename.Tests.MulliganSystemTests;

public class DeclareMulligans : MulliganSystemTest
{
  public DeclareMulligans(ITestOutputHelper output) : base(output)
  {
  }

  [Fact]
  public void Goes_in_order_of_turns()
  {
    var turnOrder = game.TurnOrder;
    var playerIds = mulliganSystem
      .DeclareMulligans()
      .Select(p => p.PlayerId)
      .ToList();

    output.WriteLine($"playerIds: [{string.Join(",", playerIds)}]");
    output.WriteLine($"turnOrder: [{string.Join(",", turnOrder)}]");

    Assert.True(Enumerable.SequenceEqual(playerIds, turnOrder));
  }
}