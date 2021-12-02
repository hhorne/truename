namespace truename.Effects;

public delegate EventDescription EventConverter(Game game, EventDescription @event);
public class ReplacementEffect : ContinuousEffect
{
  public GameCondition AppliesTo { get; private set; } = (game, @event) => false;
  public EventConverter CreateReplacement { get; private set; }

  public ReplacementEffect(GameCondition applies, EventConverter eventCreator)
    : this(applies, applies, eventCreator) { }

  public ReplacementEffect(
    GameCondition applies,
    GameCondition isExpired,
    EventConverter eventCreator
  )
    : base(isExpired)
  {
    AppliesTo = applies;
    CreateReplacement = eventCreator;
  }
}