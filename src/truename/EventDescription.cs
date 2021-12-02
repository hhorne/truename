// Something happened and we need to notify the game and/or players
public class EventDescription
{
  public Guid Id { get; set; }
  public string PlayerId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;

  public override string ToString() => Name;

  public EventDescription() { }

  public EventDescription(string message)
  {
    Name = message;
  }

  public EventDescription(string message, string description)
  {
    Name = message;
    Description = description;
  }
}