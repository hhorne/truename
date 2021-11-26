namespace truename;

public class Card
{
  // Id of the card object (i.e. Thoughtseize 1 of 4)
  public Guid Id { get; set; } = Guid.NewGuid();
  // Id of the card data (i.e. Thoughtseize)
  public string CardId { get; set; }
  public string Name { get; set; }

  public Card(string name)
  {
    CardId = name.ToLower().Replace(' ', '-');
    Name = name;
  }

  public override string ToString() => $"{Name} ({Id})";
}