namespace truename.Systems;

public class TurnSystem
{
  private readonly Game game;

  public TurnSystem(Game game)
  {
    this.game = game;
  }

  public IEnumerable<GameEvent> TakeTurn()
  {
    var activePlayerId = game.ActivePlayerId;
    // Beginning Phase
    // - Untap
    // --- The active player determines which permanents
    // --- controlled by that player untap, then untaps
    // --- all those permanents simultaneously. (The player
    // --- will untap all permanents they control unless a
    // --- card effect prevents this.)
    //
    // Requires: Untap Effect, Replacement Effect
    var untap = () => { };

    // - Upkeep
    // - Draw
    return Enumerable.Empty<GameEvent>();
  }
}