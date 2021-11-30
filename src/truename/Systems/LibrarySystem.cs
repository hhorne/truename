namespace truename.Systems;

public class LibrarySystem : System
{
  (Zones, string) libraryFor(string playerId) => (Zones.Library, playerId);

  public LibrarySystem(Game game) : base(game)
  {
  }

  public GameEvent Shuffle(string playerId)
  {
    var libraryId = libraryFor(playerId);
    var library = game
      .Zones[libraryId]
      .ToArray()
      .Shuffle()
      .ToList();

    game.UpdateZone(libraryId, library);
    return new GameEvent($"[bold darkslategray3]{game.GetPlayerName(playerId)}[/] [bold gold3_1]Shuffles[/] their [bold mediumorchid1]Library[/]");
  }

  public GameEvent TakeTop(string playerId, out IEnumerable<Card> cards, int count = 1)
  {
    var libraryId = libraryFor(playerId);
    var library = game.Zones[libraryId];
    cards = library.TakeLast(1);
    game.UpdateZone(libraryId, library.Except(cards));
    return new GameEvent
    {
      Name = $"Taking top {count} cards from {GetPlayerName(playerId)}'s Library",
      Description = string.Join(
        Environment.NewLine,
        cards.Select(c => $" - {c}")
      ),
    };
  }

  public GameEvent PutOnBottom(string playerId, IEnumerable<Card> cards)
  {
    var libraryId = libraryFor(playerId);

    game.UpdateZone(
      libraryId,
      cards.Concat(game.Zones[libraryId])
    );

    return new GameEvent($"[bold darkslategray3]{game.GetPlayerName(playerId)}[/] puts [bold gold3_1]Hand[/] on the bottom of their [bold mediumorchid1]Library[/]");
  }
}
