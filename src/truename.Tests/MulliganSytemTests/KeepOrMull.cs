using System.Linq;
using truename.Systems;
using Xunit;
using Xunit.Abstractions;

namespace truename.Tests.MulliganSystemTests;

public class KeepOrMull : MulliganSystemTest
{
  public KeepOrMull(ITestOutputHelper output)
    : base(output)
  {
  }

  [Fact]
  public void Choices_are_Keep_and_Mulligan()
  {
    var choices = mulliganSystem
      .KeepOrMull()
      .First()
      .Choices;

    Assert.NotNull(choices);
    Assert.Equal(2, choices.Length);
    Assert.Equal("Keep", choices.First().Name);
    Assert.Equal("Mulligan", choices.Last().Name);
  }

  [Fact]
  public void Keep_updates_decision()
  {
    var keep = mulliganSystem
      .KeepOrMull()
      .First()
      .Choices
      .First();

    var player = game.ActivePlayerId;
    keep.Action();

    Assert.Equal("Keep", keep.Name);
    Assert.True(mulliganSystem.Decisions[player].Keep);
  }

  [Fact]
  public void Mulligan_updates_decision()
  {
    var keep = mulliganSystem
      .KeepOrMull()
      .First()
      .Choices
      .Last();

    var player = game.ActivePlayerId;
    keep.Action();

    Assert.Equal("Mulligan", keep.Name);
    Assert.Equal(1, mulliganSystem.Decisions[player].Taken);
  }

  [Fact]
  public void Only_returns_decisions_for_undecided()
  {
    var player = game.ActivePlayerId;
    mulliganSystem.Decisions[player].Keep = true;

    var playerStillDeciding = mulliganSystem.KeepOrMull().Any(d => d.PlayerId == player);

    Assert.False(playerStillDeciding);
  }
}