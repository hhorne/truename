namespace truename.Systems;

public class HandSystem
{
  private readonly Game game;

  public HandSystem(Game game)
  {
    this.game = game;
  }

  public void Draw(Guid playerId, IEnumerable<string> cards)
  {
    var handId = (Zones.Hand, playerId);
    var hand = game.Zones[handId].Concat(cards);
    game.UpdateZone(handId, hand);
  }

  public IEnumerable<string> Take(Guid playerId, IEnumerable<string> cards)
  {
    var handId = (Zones.Hand, playerId);
    var cardsTaken = game.Zones[handId].Where(cards.Contains);
    var hand = game.Zones[handId].Except(cards);
    game.UpdateZone(handId, hand);

    return cardsTaken;
  }

  public IEnumerable<string> HandFor(Guid playerId) =>
    game.Zones[(Zones.Hand, playerId)];
}
