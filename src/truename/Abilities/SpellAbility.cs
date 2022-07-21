namespace truename.Abilities;

public abstract class SpellAbility : Ability
{
    public SpellAbility(GameEffect effect)
    {
        Effect = effect;
    }
}