namespace truename;

public class Player
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string[] DeckList { get; set; } = { };
  public List<string> Hand { get; set; } = new();
  public int TurnNumber { get; set; } = 1;
  public int LifeTotal { get; set; } = 20;
  public int DefaultHandSize { get; set; } = 7;
  public int MaximumHandSize { get; set; } = 7;

  public Player() : this(Guid.NewGuid(), string.Empty, Array.Empty<string>())
  {
  }

  public Player(string name, string[] deckList) : this(Guid.NewGuid(), name, deckList)
  {
  }

  public Player(Guid id, string name, string[] deckList)
  {
    Id = id;
    Name = name;
    DeckList = deckList;
  }
}