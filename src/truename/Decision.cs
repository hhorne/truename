namespace truename;

// Something happened and we have to get a players input
public class Decision : EventDescription
{
  public GameAction[] Choices { get; set; } = Array.Empty<GameAction>();

  public Decision() { }

  public Decision(string message) : base(message) { }

  public Decision(string message, string description)
    : base(message, description) { }
}

