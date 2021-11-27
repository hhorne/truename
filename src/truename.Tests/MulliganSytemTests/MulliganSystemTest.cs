using System.Linq;
using truename;
using truename.Systems;
using Xunit.Abstractions;

public abstract class MulliganSystemTest
{
  protected Game game;
  protected Player[] players;
  protected MulliganSystem mulliganSystem;
  protected readonly ITestOutputHelper output;

  public MulliganSystemTest(ITestOutputHelper output)
  {
    players = new[]
    {
      new Player("tron_travolta", TestData.GrixisDeck),
      new Player("typedef", TestData.ReanimatorDeck)
    };

    game = new Game(players);
    mulliganSystem = new MulliganSystem(game);
    game.SetTurnOrder(players.Select(p => p.Id));
    mulliganSystem.Init();
    this.output = output;
  }
}