using System;
using System.Linq;
using truename.Abilities;
using truename.Abilities.Common;
using truename.Characteristics;

namespace truename.Tests;

public static class ExampleCards
{
    public static readonly Card HurloonMinotaur = new Card(
        new Guid("8f1dae40-b307-446e-bbd2-86aa35813871"),
        new CardName("Hurloon Minotaur"),
        new CardColor(Colors.Red),
        new ManaCost("1|R|R"),
        new CardType("Creature"),
        new SubType("Minotaur"),
        new PowerAndToughness((2,3))
    );

    public static readonly Card LightningBolt = new Card(
        new Guid("4457ed35-7c10-48c8-9776-456485fdf070"),
        new CardName("Lightning Bolt"),
        new CardColor(Colors.Red),
        new ManaCost("R"),
        new CardType("Instant"),
        new AbilitySet(
            new BoltEffect((_, _, _, _) => Console.WriteLine("Deal 3 Damage to target"))
        )
    );

    // https://cantrip.ru/en/mtg-cards/Leylines.shtml
    public static readonly Card LeylineOfTheVoid = new Card(
        new Guid("f4e32fc1-1b8d-441e-8e76-71f19f98e925"),
        new CardName("Leyline of the Void"),
        new CardColor(Colors.Black),
        new ManaCost("2|B|B"),
        new CardType("Enchantment"),
        new AbilitySet(
            new LeylinePlayFreeAbility(
                "Put Leyline of the Void into Play",
                "If Leyline of the Void is in your opening hand, you may begin the game with it on the battlefield."
            )
        )
    );

    public static readonly Card BasicPlains= new Card(
        new Guid("bc71ebf6-2056-41f7-be35-b2e5c34afa99"),
        new CardName("Plains"),
        new CardType("Land"),
        new SuperType("Basic"),
        new SubType("Plains"),
        new AbilitySet(
            new ManaAbility((_, _, _, _) => Console.WriteLine("Add W to Mana Pool"))
        )
    );

    public static readonly Card BasicIsland = new Card(
        new Guid("b2c6aa39-2d2a-459c-a555-fb48ba993373"),
        new CardName("Island"),
        new CardType("Land"),
        new SuperType("Basic"),
        new SubType("Island"),
        new AbilitySet(
            new ManaAbility((_, _, _, _) => Console.WriteLine("Add U to Mana Pool"))
        )
    );

    public static readonly Card BasicSwamp = new Card(
        new Guid("56719f6a-1a6c-4c0a-8d21-18f7d7350b68"),
        new CardName("Swamp"),
        new CardType("Land"),
        new SuperType("Basic"),
        new SubType("Swamp"),
        new AbilitySet(
            new ManaAbility((_, _, _, _) => Console.WriteLine("Add B to Mana Pool"))
        )
    );

    public static readonly Card BasicMountain = new Card(
        new Guid("a3fb7228-e76b-4e96-a40e-20b5fed75685"),
        new CardName("Mountain"),
        new CardType("Land"),
        new SuperType("Basic"),
        new SubType("Mountain"),
        new AbilitySet(
            new ManaAbility((_, _, _, _) => Console.WriteLine("Add R to Mana Pool"))
        )
    );

    public static readonly Card BasicForest = new Card(
        new Guid("b34bb2dc-c1af-4d77-b0b3-a0fb342a5fc6"),
        new CardName("Forest"),
        new CardType("Land"),
        new SuperType("Basic"),
        new SubType("Forest"),
        new AbilitySet(
            new ManaAbility((_, _, _, _) => Console.WriteLine("Add G to Mana Pool"))
        )
    );
}
public class BoltEffect : SpellAbility
{
    public Targets Targeting { get; } = new();

    public BoltEffect(GameEffect effect)
        : base(effect) { }

    public BoltEffect(GameEffect effect, Targets targeting)
        : base(effect)
    {
        Targeting = targeting;
    }
}