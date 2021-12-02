
namespace truename.Systems;

public class TimingSystem
{
  private readonly Game game;

  public TimingSystem(Game game)
  {
    this.game = game;
  }

  public IEnumerable<EventDescription> ExchangePriority()
  {
    var priorityHolder = game.PriorityHolderId;
    var turnStep = game.TurnStep;
    var stack = game.Zones[(Zones.Stack, string.Empty)];
    return Enumerable.Empty<EventDescription>();
  }
}
