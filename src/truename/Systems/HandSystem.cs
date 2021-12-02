namespace truename.Systems;

public class HandSystem : System
{
  public HandSystem(Game game) : base(game)
  {
  }

  public void Draw(string playerId, IEnumerable<Card> cards)
  {
    var handId = (Zones.Hand, playerId);
    var hand = game.Zones[handId].Concat(cards);
    game.UpdateZone(handId, hand);
  }

  public void Remove(string playerId, IEnumerable<Card> cards)
  {
    var handId = (Zones.Hand, playerId);
    var hand = game.Zones[handId].Except(cards);
    game.UpdateZone(handId, hand);
  }

  public IEnumerable<Card> HandFor(string playerId) =>
    game.Zones[(Zones.Hand, playerId)];
}
