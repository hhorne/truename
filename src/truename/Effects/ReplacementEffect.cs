namespace truename.Effects;

public delegate GameEvent EventConverter(Game game, GameEvent @event);
public class ReplacementEffect : ContinuousEffect
{
  public GameCondition AppliesTo { get; private set; } = (game, @event) => false;
  public EventConverter CreateReplacement { get; private set; }

  public ReplacementEffect(GameCondition applies, EventConverter @event)
    : this(applies, applies, @event) { }

  public ReplacementEffect(GameCondition applies, GameCondition isExpired, EventConverter @event)
    : base(isExpired)
  {
    this.AppliesTo = applies;
    this.CreateReplacement = @event;
  }
}