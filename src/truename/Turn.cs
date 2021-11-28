namespace truename;

public class Turn
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
}