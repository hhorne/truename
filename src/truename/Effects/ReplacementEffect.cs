namespace truename.Effects;

public class ReplacementEffect : ContinuousEffect
{
  public GameCondition AppliesTo { get; private set; } = (game, @event) => false;
  public IEnumerable<GameEvent> Events { get; private set; } = Enumerable.Empty<GameEvent>();


  public ReplacementEffect(GameCondition applies, IEnumerable<GameEvent> events)
    : this(applies, applies, events) { }

  public ReplacementEffect(GameCondition applies, GameCondition expires, IEnumerable<GameEvent> events)
    : base(expires)
  {
    this.AppliesTo = applies;
    this.Events = events;
  }
}