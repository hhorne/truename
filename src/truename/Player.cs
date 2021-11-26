namespace truename;

public class Player
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Card[] DeckList { get; set; } = { };
  public List<string> Hand { get; set; } = new();
  public int TurnNumber { get; set; } = 1;
  public int LifeTotal { get; set; } = 20;
  public int DefaultHandSize { get; set; } = 7;
  public int MaximumHandSize { get; set; } = 7;

  public Player() : this(Guid.NewGuid(), string.Empty, Array.Empty<Card>())
  {
  }

  public Player(string name, Card[] deckList) : this(Guid.NewGuid(), name, deckList)
  {
  }

  public Player(Guid id, string name, Card[] deckList)
  {
    Id = id;
    Name = name;
    DeckList = deckList;
  }
}