namespace truename.Effects;

public delegate bool GameCondition(Game game, EventDescription @event);

public class ContinuousEffect
{
  GameCondition isExpired = (game, @event) => false;
  public bool IsExpired(Game game, EventDescription @event) => isExpired(game, @event);

  public ContinuousEffect(GameCondition expires)
  {
    this.isExpired = expires;
  }
}