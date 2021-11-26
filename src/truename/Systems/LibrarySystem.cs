namespace truename.Systems;

public class LibrarySystem
{
  private readonly Game game;
  (Zones, Guid?) libraryFor(Guid playerId) => (Zones.Library, playerId);

  public LibrarySystem(Game game)
  {
    this.game = game;
  }

  public GameEvent Shuffle(Guid playerId)
  {
    var libraryId = libraryFor(playerId);
    var library = game
      .Zones[libraryId]
      .ToArray()
      .Shuffle()
      .ToList();

    game.UpdateZone(libraryId, library);
    return new GameEvent($"{game.GetPlayerName(playerId)} [Shuffle]s their [Library]");
  }

  public IEnumerable<string> TakeTop(Guid playerId, int count = 1)
  {
    var libraryId = libraryFor(playerId);
    var library = game.Zones[libraryId];
    var toDraw = library.TakeLast(1);
    game.UpdateZone(libraryId, library.Except(toDraw));
    return toDraw;
  }

  public GameEvent PutOnBottom(Guid playerId, IEnumerable<string> cards)
  {
    var libraryId = libraryFor(playerId);

    game.UpdateZone(
      libraryId,
      cards.Concat(game.Zones[libraryId])
    );

    return new GameEvent($"{game.GetPlayerName(playerId)} puts [Hand] on the bottom of their [Library]");
  }
}
