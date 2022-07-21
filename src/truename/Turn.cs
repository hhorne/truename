using System.Collections.ObjectModel;

namespace truename;

public static class Turn
{
    public static class PreGame
    {
        public static readonly string WaitingForPlayers = $"{nameof(PreGame)}/{nameof(WaitingForPlayers)}";
        public static readonly string DetermineTurnOrder = $"{nameof(PreGame)}/{nameof(DetermineTurnOrder)}";
        public static readonly string DrawOpeningHands = $"{nameof(PreGame)}/{nameof(DrawOpeningHands)}";
        public static readonly string PreGameActions = $"{nameof(PreGame)}/{nameof(PreGameActions)}";
    }

    public static class Steps
    {
        public static readonly string Untap = $"Turn/Beginning/{nameof(Untap)}";
        public static readonly string Upkeep = $"Turn/Beginning/{nameof(Upkeep)}";
        public static readonly string Draw = $"Turn/Beginning/{nameof(Draw)}";
        public static readonly string Main = "Turn/Main";
        public static readonly string BeginCombat = $"Turn/Combat/{nameof(BeginCombat)}";
        public static readonly string DeclareAttackers = $"Turn/Combat/{nameof(DeclareAttackers)}";
        public static readonly string DeclareBlockers = $"Turn/Combat/{nameof(DeclareBlockers)}";
        public static readonly string CombatDamage = $"Turn/Combat/{nameof(CombatDamage)}";
        public static readonly string EndCombat = $"Turn/Combat/{nameof(EndCombat)}";
        public static readonly string EndStep = $"Turn/Ending/{nameof(EndStep)}";
        public static readonly string Cleanup = $"Turn/Ending/{nameof(Cleanup)}";
    }

    public static class Phases
    {
        public static readonly string[] Beginning = new string[]
        {
            Steps.Untap,
            Steps.Upkeep,
            Steps.Draw,
        };

        public static readonly string[] Combat = new string[]
        {
            Steps.BeginCombat,
            Steps.DeclareAttackers,
            Steps.DeclareBlockers,
            Steps.CombatDamage,
            Steps.EndCombat,
        };

        public static readonly string[] Ending = new string[]
        {
            Steps.Cleanup,
            Steps.EndStep,
        };
    }

    public static string[] StepsWithPriorityExchange =
    {
        Steps.Upkeep,
        Steps.Draw,
        Steps.Main,
        Steps.BeginCombat,
        Steps.DeclareAttackers,
        Steps.DeclareBlockers,
        Steps.CombatDamage,
        Steps.EndCombat,
        Steps.EndStep
    };

    public static ReadOnlyCollection<string> Structure { get; } = new(new[]
    {
        Steps.Untap,
        Steps.Upkeep,
        Steps.Draw,
        Steps.Main,
        Steps.BeginCombat,
        Steps.DeclareAttackers,
        Steps.DeclareBlockers,
        Steps.CombatDamage,
        Steps.EndCombat,
        Steps.Main,
        Steps.Cleanup,
        Steps.EndStep
  });
}