using System;
using System.Linq;
using truename.Effects;
using truename.Effects.Common;
using truename.Events;
using Xunit;

namespace truename.Tests;

public class ReplacementEffectsTests
{
    public class SkipFirstDrawTests
    {
        Player testPlayer = new Player("tron_travolta", new[] { ExampleCards.BasicMountain });
        SkipFirstDraw SkipFirstDraw = new SkipFirstDraw();

        [Fact]
        public void AppliesToFirstDraw()
        {
            var match = new Match(Guid.NewGuid())
            {
                ActivePlayerId = testPlayer.Id,
                CurrentStep = Turn.Steps.Draw,
                Players = new()
                {
                    { testPlayer.Id, testPlayer },
                },
                TurnOrder = new[] { testPlayer.Id },
            };

            var applies = SkipFirstDraw.AppliesTo(match, new Draw(testPlayer.Id));

            Assert.True(applies);
        }

        [Fact]
        public void ExpiresAfterFirstDraw()
        {
            var match = new Match(Guid.NewGuid())
            {
                ActivePlayerId = testPlayer.Id,
                CurrentStep = Turn.Steps.Draw,
                Players = new()
                {
                    { testPlayer.Id, new Player(testPlayer.Id, new[] { ExampleCards.BasicMountain }) },
                },
                TurnOrder = new[] { testPlayer.Id },
            };

            var expired = SkipFirstDraw.IsExpired(match, new Draw(testPlayer.Id));

            Assert.True(expired);
        }
    }
}
