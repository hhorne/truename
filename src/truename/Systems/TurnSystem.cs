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
    var playerId = game.ActivePlayerId;
    var activePlayer = game.Players[playerId];
    if (activePlayer is null)
      throw new KeyNotFoundException();
    var playerName = activePlayer.Name;

    // Beginning Phase
    // - Untap
    // --- The active player determines which permanents
    // --- controlled by that player untap, then untaps
    // --- all those permanents simultaneously. (The player
    // --- will untap all permanents they control unless a
    // --- card effect prevents this.)
    //
    // Requires: Untap Effect, Replacement Effect
    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Untap Step",
      Type = "Turn/Step/Untap",
      Actions = new[]
      {
        new GameAction("Untap", () => {}),
      },
    };

    // - Upkeep
    // - Draw
    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Draw Step",
      Type = "Turn/Step/Draw",
      Actions = new[]
      {
        new GameAction("Draw", () => {}),
      },
    };

    var turnOrder = game.TurnOrder;
    var activePlayerId = game.ActivePlayerId;
    var nextActivePlayerId = turnOrder.After(activePlayerId);
    game.UpdateActivePlayer(nextActivePlayerId);
  }
}