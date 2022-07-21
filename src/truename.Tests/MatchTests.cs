using System;
using System.Collections.Generic;
using System.Linq;
using truename.Events;
using Xunit;

namespace truename.Tests;

public class MatchTests
{

    public static Card[] SimpleDeck = 
        4.Of(() => ExampleCards.LightningBolt)
        .Concat(4.Of(() => ExampleCards.BasicMountain))
        .ToArray();

    public static readonly PlayerJoined player1Joined = new PlayerJoined(new Player("tron_travolta", SimpleDeck));
    public static readonly PlayerJoined player2Joined = new PlayerJoined(new Player("typedef", SimpleDeck));
    public static readonly PlayerJoined extraPlayerJoined = new PlayerJoined(new Player("extra", SimpleDeck));

    public class JoinPlayer
    {

        [Fact]
        public void StatusRemainsWaitingForPlayersWhenNotAllPlayersJoined()
        {
            var match = new Match(Guid.NewGuid());
            
            match.Apply(player1Joined);

            Assert.Equal(Turn.PreGame.WaitingForPlayers, match.CurrentStep);
        }

        [Fact]
        public void StatusChangesToDetermineTurnOrderAfterAllPlayersJoin()
        {
            var match = new Match(Guid.NewGuid());

            match.Apply(player1Joined);
            match.Apply(player2Joined);

            Assert.Equal(Turn.PreGame.DetermineTurnOrder, match.CurrentStep);
        }

        [Fact]
        public void ThrowsExceptionIfExtraPlayersTryToJoin()
        {
            var match = new Match(Guid.NewGuid());

            match.Apply(player1Joined);
            match.Apply(player2Joined);

            Assert.Throws<Exception>(
                () => match.Apply(extraPlayerJoined)
            );
        }
    }

    public class SetTurnOrderTests
    {
        [Fact]
        public void ThrowsExceptionIfCalledBeforeAllPlayersJoined()
        {
            var match = new Match(Guid.NewGuid());

            Assert.Throws<Exception>(
                () => match.Apply(new SetTurnOrder(Array.Empty<string>()))
            );
        }

        [Fact]
        public void ThrowsExceptionIfInvalidPlayersAreIncluded()
        {
            var match = new Match(Guid.NewGuid())
            {
                CurrentStep = Turn.PreGame.DetermineTurnOrder,
            };

            Assert.Throws<Exception>(
                () => match.Apply(new SetTurnOrder(new[] { "test" }))
            );
        }

        [Fact]
        public void MovesToDrawOpeningHandsAfterward()
        {
            var match = new Match(Guid.NewGuid());

            match.Apply(player1Joined);
            match.Apply(player2Joined);

            match.Apply(new SetTurnOrder(new []
            {
                player1Joined.Player.Id,
                player2Joined.Player.Id
            }));

            Assert.Equal(Turn.PreGame.DrawOpeningHands, match.CurrentStep);
        }
    }
}