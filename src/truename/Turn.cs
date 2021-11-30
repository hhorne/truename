namespace truename;

public class Turn
{
  public class Phases
  {
    public static readonly string BaseKey = "Turn/Phase/";
    // beginning, precombat main, combat, postcombat main, and ending
    public static readonly string Beginning = $"{BaseKey}{nameof(Beginning)}";
    public static readonly string PreCombatMain = $"{BaseKey}{nameof(PreCombatMain)}";
    public static readonly string Combat = $"{BaseKey}{nameof(Combat)}";
    public static readonly string PostCombatMain = $"{BaseKey}{nameof(PostCombatMain)}";
    public static readonly string Ending = $"{BaseKey}{nameof(Ending)}";
  }

  public class Steps
  {
    public static readonly string BaseKey = "Turn/Step/";
    public static readonly string Untap = $"{BaseKey}{nameof(Untap)}";
    public static readonly string Upkeep = $"{BaseKey}{nameof(Upkeep)}";
    public static readonly string Draw = $"{BaseKey}{nameof(Draw)}";
    public static readonly string BeginCombat = $"{BaseKey}{nameof(BeginCombat)}";
    public static readonly string DeclareAttackers = $"{BaseKey}{nameof(DeclareAttackers)}";
    public static readonly string DeclareBlockers = $"{BaseKey}{nameof(DeclareBlockers)}";
    public static readonly string CombatDamage = $"{BaseKey}{nameof(CombatDamage)}";
    public static readonly string EndCombat = $"{BaseKey}{nameof(EndCombat)}";
    public static readonly string EndStep = $"{BaseKey}{nameof(EndStep)}";
    public static readonly string Cleanup = $"{BaseKey}{nameof(Cleanup)}";
  }

  public static string[] PartsWithPriority =
  {
    Steps.Upkeep,
    Steps.Draw,
    Phases.PreCombatMain,
    Steps.BeginCombat,
    Steps.DeclareAttackers,
    Steps.DeclareBlockers,
    Steps.CombatDamage,
    Steps.EndCombat,
    Phases.PostCombatMain,
    Steps.EndStep
  };
}