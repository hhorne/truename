namespace truename.Systems;

public class TurnSystem
{
  private readonly Game game;

  public TurnSystem(Game game)
  {
    this.game = game;
  }

  public IEnumerable<EventDescription> TakeTurn()
  {
    var activePlayer = game.ActivePlayer;

    foreach (var @event in Beginning())
      yield return @event;

    foreach (var @event in PreCombatMain())
      yield return @event;

    foreach (var @event in Combat())
      yield return @event;

    foreach (var @event in PostCombatMain())
      yield return @event;

    foreach (var @event in Ending())
      yield return @event;

    // Convert below to a Next Players Turn Event
    var turnOrder = game.TurnOrder;
    var activePlayerId = game.ActivePlayerId;
    var nextActivePlayerId = turnOrder.After(activePlayerId);
    game.UpdateActivePlayer(nextActivePlayerId);
  }

  public IEnumerable<EventDescription> Beginning()
  {
    var activePlayer = game.ActivePlayer;
    var playerId = activePlayer.Id;
    var playerName = activePlayer.Name;

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Beginning Phase",
      Type = Turn.Phases.Beginning,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Untap Step",
      Type = Turn.Steps.Untap,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Upkeep",
      Type = Turn.Steps.Upkeep,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Draw Step",
      Type = Turn.Steps.Draw,
    };
  }

  public IEnumerable<EventDescription> PreCombatMain()
  {
    var activePlayer = game.ActivePlayer;
    var playerId = activePlayer.Id;
    var playerName = activePlayer.Name;

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Pre-Combat Main Phase",
      Description = "[white italic] - Play lands and cast spells[/]",
      Type = Turn.Phases.PreCombatMain,
    };
  }

  public IEnumerable<EventDescription> Combat()
  {
    var activePlayer = game.ActivePlayer;
    var playerId = activePlayer.Id;
    var playerName = activePlayer.Name;

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Combat Phase",
      Type = Turn.Phases.Combat,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}: Beginning of Combat",
      Type = Turn.Steps.BeginCombat,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}: Declare Attackers",
      Type = Turn.Steps.DeclareAttackers,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}: Declare Blockers",
      Type = Turn.Steps.DeclareBlockers,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}: Combat Damage",
      Type = Turn.Steps.CombatDamage,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s End of Combat",
      Type = Turn.Steps.EndCombat,
    };
  }
  
  public IEnumerable<EventDescription> PostCombatMain()
  {
    var activePlayer = game.ActivePlayer;
    var playerId = activePlayer.Id;
    var playerName = activePlayer.Name;

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Post-Combat Main Phase",
      Description = "[white italic] - Play lands and cast spells[/]",
      Type = Turn.Phases.PostCombatMain,
    };
  }

  public IEnumerable<EventDescription> Ending()
  {
    var activePlayer = game.ActivePlayer;
    var playerId = activePlayer.Id;
    var playerName = activePlayer.Name;

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Ending Phase",
      Type = Turn.Phases.Ending,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s End Step",
      Type = Turn.Steps.EndStep,
    };

    yield return new EventDescription
    {
      PlayerId = playerId,
      Name = $"{playerName}'s Cleanup Step",
      Type = Turn.Steps.Cleanup,
    };
  }
}