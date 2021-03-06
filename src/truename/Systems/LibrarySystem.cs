namespace truename.Systems;

public class LibrarySystem
{
  private readonly Game game;
  (Zones, string) libraryFor(string playerId) => (Zones.Library, playerId);

  public LibrarySystem(Game game)
  {
    this.game = game;
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

  public IEnumerable<Card> TakeTop(string playerId, int count = 1)
  {
    var libraryId = libraryFor(playerId);
    var library = game.Zones[libraryId];
    var toDraw = library.TakeLast(1);
    game.UpdateZone(libraryId, library.Except(toDraw));
    return toDraw;
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
