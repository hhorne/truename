namespace truename;

public class GameAction
{
  public Guid Id { get; set; }
  public string PlayerId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public Action Action { get; set; }

  public override string ToString() => Name;

  public GameAction(string name, Action action)
  {
    Name = name;
    Action = action;
  }
}