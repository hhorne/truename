namespace truename.Effects;

public delegate IGameEvent EventConverter<T>(Game game, T @event);
public delegate bool GameCondition<T>(Game game, T @event);

public class ReplacementEffect<T> : ContinuousEffect where T : IGameEvent
{
  public GameCondition<T> AppliesTo { get; private set; }
  public GameCondition<T> IsExpired { get; private set; }
  public EventConverter<T> CreateReplacement { get; private set; }

  public ReplacementEffect(
    GameCondition<T> appliesTo,
    GameCondition<T> isExpired,
    EventConverter<T> eventCreator
  )
  {
    AppliesTo = appliesTo;
    CreateReplacement = eventCreator;
    IsExpired = isExpired;
  }

  // ForOne-Shot Effects, expire as soon as they're applied.
  public ReplacementEffect(
    GameCondition<T> applies,
    EventConverter<T> eventCreator
  ) : this(
    applies,
    applies,
    eventCreator
  ) { }
}