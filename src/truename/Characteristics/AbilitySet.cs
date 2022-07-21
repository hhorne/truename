using truename.Abilities;

namespace truename.Characteristics;

public class AbilitySet : Characteristic<IAbility[]>
{
    public AbilitySet(params IAbility[] value) : base(value) { }
    public IEnumerable<T> OfType<T>() => Value.OfType<T>();
}