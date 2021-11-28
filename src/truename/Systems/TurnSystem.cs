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
    var activePlayer = game.ActivePlayer;
    var playerId = activePlayer.Id;
    var playerName = activePlayer.Name;

    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Untap Step",
      Type = Turn.Untap,
    };

    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Upkeep",
      Type = Turn.Upkeep,
    };

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