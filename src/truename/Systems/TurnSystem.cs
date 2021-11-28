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
    var activePlayer = game.ActivePlayer;
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
      Type = Turn.Untap,
    };

    // - Upkeep
    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Upkeep",
      Type = Turn.Upkeep,
    };

    // - Draw
    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Draw Step",
      Type = Turn.Draw,
    };

    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Pre-Combat Main Phase",
      Description = "[white italic] - Play lands and cast spells[/]",
      Type = Turn.PreCombatMain,
    };

    var turnOrder = game.TurnOrder;
    var activePlayerId = game.ActivePlayerId;
    var nextActivePlayerId = turnOrder.After(activePlayerId);
    game.UpdateActivePlayer(nextActivePlayerId);
  }
}