
namespace truename.Systems;

public class TimingSystem
{
  private readonly Game game;

  public TimingSystem(Game game)
  {
    this.game = game;
  }

  IEnumerable<GameEvent> ExchangePriority()
  {    
    return Enumerable.Empty<GameEvent>();
  }
}
