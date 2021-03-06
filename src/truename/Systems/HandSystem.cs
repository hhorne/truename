namespace truename.Systems;

public class HandSystem
{
  private readonly Game game;

  public HandSystem(Game game)
  {
    this.game = game;
  }

  public void Draw(string playerId, IEnumerable<Card> cards)
  {
    var handId = (Zones.Hand, playerId);
    var hand = game.Zones[handId].Concat(cards);
    game.UpdateZone(handId, hand);
  }

  public IEnumerable<Card> Take(string playerId, IEnumerable<Card> cards)
  {
    var handId = (Zones.Hand, playerId);
    var cardsTaken = game.Zones[handId].Where(cards.Contains);
    var hand = game.Zones[handId].Except(cards);
    game.UpdateZone(handId, hand);

    return cardsTaken;
  }

  public IEnumerable<Card> HandFor(string playerId) =>
    game.Zones[(Zones.Hand, playerId)];
}
