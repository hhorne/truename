using truename;

public class GameEvent
{
  public Guid Id { get; set; }
  public Guid? PlayerId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public GameAction[] Options { get; set; } = { };
  public override string ToString() => Name;

  public GameEvent() { }

  public GameEvent(string message)
  {
    Name = message;
  }
}