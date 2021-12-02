namespace truename.Systems;

public class LibrarySystem : System
{
  (Zones, string) libraryFor(string playerId) => (Zones.Library, playerId);

  public LibrarySystem(Game game) : base(game)
  {
  }

  public void Shuffle(string playerId)
  {
    var libraryId = libraryFor(playerId);
    var library = game
      .Zones[libraryId]
      .ToArray()
      .Shuffle()
      .ToList();

    game.UpdateZone(libraryId, library);
  }

  public IEnumerable<Card> TakeTop(string playerId, int count = 1)
  {
    var libraryId = libraryFor(playerId);
    var library = game.Zones[libraryId];
    var cards = library.TakeLast(1);
    game.UpdateZone(libraryId, library.Except(cards));
    return cards;
  }

  public void PutOnBottom(string playerId, IEnumerable<Card> cards)
  {
    var libraryId = libraryFor(playerId);
    game.UpdateZone(
      libraryFor(playerId),
      cards.Concat(game.Zones[libraryId])
    );
  }
}
