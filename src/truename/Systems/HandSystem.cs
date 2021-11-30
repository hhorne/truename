namespace truename.Systems;

public class HandSystem : System
{
  public HandSystem(Game game) : base(game)
  {
  }

  public GameEvent Draw(string playerId, IEnumerable<Card> cards)
  {
    var handId = (Zones.Hand, playerId);
    var hand = game.Zones[handId].Concat(cards);
    game.UpdateZone(handId, hand);

    return new GameEvent($"{GetPlayerName(playerId)} drew {cards.First()}");
  }

  public GameEvent Take(string playerId, IEnumerable<Card> toTake, out IEnumerable<Card> result)
  {
    var handId = (Zones.Hand, playerId);
    var hand = game.Zones[handId].Except(toTake);
    result = game.Zones[handId].Where(toTake.Contains);
    game.UpdateZone(handId, hand);

    return new GameEvent($"Taking cards from {GetPlayerName(playerId)}'s Hand");
  }

  public IEnumerable<Card> HandFor(string playerId) =>
    game.Zones[(Zones.Hand, playerId)];
}
