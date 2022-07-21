namespace truename.Abilities;

public abstract class Ability : IAbility
{
    public GameEffect Effect { get; set; } = (_,_,_,_) => { };
    public GameCondition[] Conditions { get; set; } = Array.Empty<GameCondition>();
    public string Label { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    public virtual void Resolve(Match m, Guid sourceId, string controllerId, string ownerId)
    {
        var allConditionsMet = Conditions.All(c => c(m, sourceId, controllerId));
        if (allConditionsMet)
        {
            Effect(m, sourceId, controllerId);
        }
    }
}
