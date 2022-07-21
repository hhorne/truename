using System;
using System.Collections.Generic;
using System.Linq;
using truename.Characteristics;
using Xunit;

namespace truename.Tests;

public class LeylinePlayFreeAbilityTests
{
    public class Resolve
    {
        [Fact]
        public void MovesCardToBattlefieldFromHandPreGame()
        {
            var playerId = "tron_travolta";
            var lotv = ExampleCards.LeylineOfTheVoid;
            var match = new Match(Guid.NewGuid())
            {
                CurrentStep = Turn.PreGame.PreGameActions,
                Players = new()
                {
                    { playerId, new Player(playerId, Array.Empty<Card>()) },
                },
                Zones = new()
                {
                    { (ZoneTypes.Battlefield, null), new List<Card>() },
                    { (ZoneTypes.Hand, playerId), new List<Card> { lotv } },
                }
            };

            var battlefield = match.Zones[(ZoneTypes.Battlefield, null)];
            var leylineAbility = lotv
                .GetCharacteristic<AbilitySet>()
                    ?.Value
                    .First(ability => ability
                        .Conditions
                            .All(c => c(match, lotv.ZonedId, playerId))
                    );

            leylineAbility?.Resolve(match, lotv.ZonedId, playerId);

            Assert.Contains(battlefield, c => c.Id == lotv.Id);
        }

        [Fact]
        public void CantBeActivatedFromAnywhereElse()
        {
            var playerId = "tron_travolta";
            var lotv = ExampleCards.LeylineOfTheVoid;
            var match = new Match(Guid.NewGuid())
            {
                CurrentStep = Turn.PreGame.PreGameActions,
                Players = new()
                {
                    { playerId, new Player(playerId, Array.Empty<Card>()) },
                },
                Zones = new()
                {
                    { (ZoneTypes.Hand, playerId), new List<Card>() },
                    { (ZoneTypes.Battlefield, null), new List<Card> { lotv } },
                }
            };

            var battlefield = match.Zones[(ZoneTypes.Battlefield, null)];
            var leylineOnBattlefield = battlefield.Single(c => c.Id == lotv.Id);
            var leylineAbility = lotv
                .GetCharacteristic<AbilitySet>()
                    ?.Value
                    .FirstOrDefault(ability => ability
                        .Conditions
                            .All(c => c(match, lotv.ZonedId, playerId))
                    );


            Assert.Null(leylineAbility);
        }
    }
}