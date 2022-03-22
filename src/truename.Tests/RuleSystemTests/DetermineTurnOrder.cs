using Xunit.Abstractions;
using System.Linq;
using Xunit;
using System;
using truename.Events;

namespace truename.Tests.RuleSystemTests;

public class DetermineTurnOrder
{
  protected Game game;
  protected Player[] players;
  protected RuleSystem ruleSystem;
  protected readonly ITestOutputHelper output;

  public DetermineTurnOrder(ITestOutputHelper output)
  {
    players = new[]
    {
      new Player("tron_travolta", TestData.GrixisDeck),
      new Player("typedef", TestData.ReanimatorDeck)
    };

    game = new Game(players);
    ruleSystem = new RuleSystem(game);
    this.output = output;
  }

  [Fact]
  public void Throws_if_no_Choice_made()
  {
    var process = ruleSystem.DetermineTurnOrder();
    Assert.Throws<Exception>(() => process.Last());
  }

  [Fact]
  public void First_yields_a_Decision()
  {
    var process = ruleSystem.DetermineTurnOrder();
    var @event = process.First();

    Assert.IsType<Decision>(@event);
  }

  [Fact]
  public void Yields_SetTurnOrder_Event_after_Decision()
  {
    var process = ruleSystem.DetermineTurnOrder();
    var enumerator = process.GetEnumerator();
    enumerator.MoveNext();
    
    var decision = enumerator.Current as Decision;
    decision?.Choices
      .Single(c => c.Name == "Play")
      .Action();

    enumerator.MoveNext();

    Assert.IsType<SetTurnOrder>(enumerator.Current);
  }

  [Fact]
  public void Choosing_Play_puts_Winner_first_in_TurnOrder()
  {
    var process = ruleSystem.DetermineTurnOrder();
    var enumerator = process.GetEnumerator();
    enumerator.MoveNext();

    var decision = enumerator.Current as Decision;
    decision?.Choices
      .Single(c => c.Name == "Play")
      .Action();

    enumerator.MoveNext();
    var @event = enumerator.Current as SetTurnOrder;
    Assert.Equal(@event?.PlayerId, game.TurnOrder.First());
  }

  [Fact]
  public void Choosing_Draw_puts_Winner_last_in_TurnOrder()
  {
    var process = ruleSystem.DetermineTurnOrder();
    var enumerator = process.GetEnumerator();
    enumerator.MoveNext();

    var decision = enumerator.Current as Decision;
    decision?.Choices
      .Single(c => c.Name == "Draw")
      .Action();

    enumerator.MoveNext();
    var @event = enumerator.Current as SetTurnOrder;
    Assert.Equal(@event?.PlayerId, game.TurnOrder.Last());
  }
}
