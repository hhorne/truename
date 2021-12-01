// Something happened and we need to notify the game and/or players
public class GameEvent
{
  public Guid Id { get; set; }
  public string PlayerId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;

  public override string ToString() => Name;

  public GameEvent() { }

  public GameEvent(string message)
  {
    Name = message;
  }

  public GameEvent(string message, string description)
  {
    Name = message;
    Description = description;
  }
}