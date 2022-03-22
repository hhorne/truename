namespace truename.Events;

public record UpdateTurnStep(string TurnStep) : IGameEvent
{
  public string Name => "Update Turn Step";
  public override string ToString() => $"{Name}: {TurnStep}";

  public void Resolve(Game g)
  {
    g.TurnStep = TurnStep;
  }
}