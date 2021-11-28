using System.Linq;
using truename.Systems;
using Xunit;
using Xunit.Abstractions;

namespace truename.Tests;
public class ExchangePriorityTests
{
  protected Game game;
  protected Player[] players;
  protected TimingSystem timingSystem;
  protected readonly ITestOutputHelper output;

  public ExchangePriorityTests(ITestOutputHelper output)
  {
    players = new[]
    {
      new Player("tron_travolta", TestData.GrixisDeck),
      new Player("typedef", TestData.ReanimatorDeck)
    };

    game = new Game(players);
    timingSystem = new TimingSystem(game);
    game.SetTurnOrder(players.Select(p => p.Id));
    this.output = output;
  }

  [Fact]
  void Starts_with_event_for_Active_Player()
  {
    var @event = timingSystem.ExchangePriority().First();
    Assert.NotNull(@event);
    Assert.Equal(players.First().Id, game.ActivePlayerId);
  }

}
