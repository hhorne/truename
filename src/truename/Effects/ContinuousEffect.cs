namespace truename.Effects;

public delegate bool GameCondition(Game game, GameEvent @event);

public class ContinuousEffect
{
  GameCondition expires = (game, @event) => false;
  public bool Expired(Game game, GameEvent @event) => expires(game, @event);

  public ContinuousEffect(GameCondition expires)
  {
    this.expires = expires;
  }
}