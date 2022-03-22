namespace truename.Events;

public record Skip(IGameEvent @event) : IGameEvent
{
  public string Name => "Skip";
  public override string ToString() => $"{Name}: {@event}";

  public void Resolve(Game g) { }
}

