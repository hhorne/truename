namespace truename.Events;

public record SetTurnOrder(string PlayerId, IEnumerable<string> TurnOrder) : IGameEvent
{
  public string Name => "Set Turn Order";

  public void Resolve(Game g)
  {
    g.TurnOrder = TurnOrder.ToList();
    g.ActivePlayerId = TurnOrder.First();
  }
}
