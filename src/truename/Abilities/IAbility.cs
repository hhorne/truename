namespace truename.Abilities;

using PlayerId = String;

public interface IAbility
{
    GameCondition[] Conditions { get; set; }
    // Short, presented to user in options
    string Label { get; set; }
    // Full rules text
    string Text { get; set; }
    void Resolve(Match g, Guid sourceId = new(), PlayerId controllerId = "", PlayerId ownerId = "");
}
