using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace truename.Tests;

public class DetermineTurnOrder
{
  protected Game game;
  protected Player[] players;
  protected readonly ITestOutputHelper output;

  public DetermineTurnOrder(ITestOutputHelper output)
  {
    players = new[]
    {
      new Player("tron_travolta", TestData.GrixisDeck),
      new Player("typedef", TestData.ReanimatorDeck)
    };

    game = new Game(players);
    this.output = output;
  }
  public void TurnOrderStuff()
  {

  }
}
