namespace truename.Events;

public record Draw(string PlayerId) : IGameEvent
{
  public string Name => "Draw";

  public void Resolve(Game g)
  {
    var playerId = PlayerId;
    var library = g.Zones[(Zones.Library, playerId)];
    var hand = g.Zones[(Zones.Hand, playerId)];
    var cards = library.TakeLast(1);
    g.UpdateZone((Zones.Library, playerId), library.Except(cards));
    g.UpdateZone((Zones.Hand, playerId), hand.Concat(cards));
  }
}