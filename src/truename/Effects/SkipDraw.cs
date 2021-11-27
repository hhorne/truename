namespace truename.Effects;

public class SkipDraw : ReplacementEffect
{
  static readonly GameEvent events = new GameEvent
  {
    Name = "Skip Draw",
    Type = $"Skip/Draw"
  };

  public SkipDraw(GameCondition applies)
    : base(applies, events) { }

  public SkipDraw(GameCondition applies, GameCondition expires)
    : base(applies, expires, events) { }
}