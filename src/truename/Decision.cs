namespace truename;

// Something happened and we have to get a players input
public class Decision : IGameEvent
{
  public string PlayerId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public GameAction[] Choices { get; set; } = Array.Empty<GameAction>();
  public string[] Selections { get; set; } = Array.Empty<string>();

  public Decision() { }

  public Decision(string name) : this(name, string.Empty) { }

  public Decision(string name, string description)
  {
    Name = name;
    Description = description;
  }

  public void Resolve(Game g)
  {
    var choices = Choices
      .Select(choice => choice.Name)
      .Where(Selections.Contains)
      .ToArray();
  }
}

