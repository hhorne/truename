namespace truename.Effects;

public class ReplacementEffect : ContinuousEffect
{
  public GameCondition AppliesTo { get; private set; } = (game, @event) => false;
  public GameEvent Event { get; private set; }


  public ReplacementEffect(GameCondition applies, GameEvent @event)
    : this(applies, applies, @event) { }

  public ReplacementEffect(GameCondition applies, GameCondition expires, GameEvent @event)
    : base(expires)
  {
    this.AppliesTo = applies;
    this.Event = @event;
  }
}