namespace truename.Systems;

public class TurnSystem
{
  private static readonly string BaseEventKey = "Turn/Step/";
  public static readonly string Untap = $"{BaseEventKey}{nameof(Untap)}";
  public static readonly string Upkeep = $"{BaseEventKey}{nameof(Upkeep)}";
  public static readonly string Draw = $"{BaseEventKey}{nameof(Draw)}";
  public static readonly string PreCombatMain = $"{BaseEventKey}{nameof(PreCombatMain)}";
  public static readonly string BeginCombat = $"{BaseEventKey}{nameof(BeginCombat)}";
  public static readonly string DeclareAttackers = $"{BaseEventKey}{nameof(DeclareAttackers)}";
  public static readonly string DeclareBlockers = $"{BaseEventKey}{nameof(DeclareBlockers)}";
  public static readonly string CombatDamage = $"{BaseEventKey}{nameof(CombatDamage)}";
  public static readonly string EndCombat = $"{BaseEventKey}{nameof(EndCombat)}";
  public static readonly string PostCombatMain = $"{BaseEventKey}{nameof(PostCombatMain)}";
  public static readonly string EndStep = $"{BaseEventKey}{nameof(EndStep)}";
  public static readonly string Cleanup = $"{BaseEventKey}{nameof(Cleanup)}";

  public static readonly string[] BeginningPhase = new[]
  {
    Untap,
    Upkeep,
    Draw
  };

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
      Type = Untap,
    };

    // - Upkeep
    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Upkeep",
      Type = Upkeep,
    };

    // - Draw
    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Draw Step",
      Type = Draw,
    };

    yield return new GameEvent
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Pre-Combat Main Phase",
      Description = "[white italic] - Play lands and cast spells[/]",
      Type = PreCombatMain,
    };

    var turnOrder = game.TurnOrder;
    var activePlayerId = game.ActivePlayerId;
    var nextActivePlayerId = turnOrder.After(activePlayerId);
    game.UpdateActivePlayer(nextActivePlayerId);
  }
}