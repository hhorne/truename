using truename.Events;

namespace truename.Effects;

public delegate IGameEvent EventConverter<T>(Match match, T @event);
public delegate bool EventCondition<T>(Match match, T @event);

public class ContinuousEffect { }

public class ReplacementEffect<T> : ContinuousEffect where T : IGameEvent
{
    public EventCondition<T> AppliesTo { get; set; } = (m, e) => false;
    public EventCondition<T> IsExpired { get; set; } = (m, e) => false;
    public EventConverter<T> CreateReplacement { get; set; } = (m, e) => throw new NotImplementedException();

    public ReplacementEffect() { }

    public ReplacementEffect(
      EventCondition<T> appliesTo,
      EventCondition<T> isExpired,
      EventConverter<T> eventCreator
    )
    {
        AppliesTo = appliesTo;
        CreateReplacement = eventCreator;
        IsExpired = isExpired;
    }

    // ForOne-Shot Effects, expire as soon as they're applied.
    public ReplacementEffect(
        EventCondition<T> applies,
        EventConverter<T> eventCreator
    ) : this(
        applies,
        applies,
        eventCreator
    ) { }
}