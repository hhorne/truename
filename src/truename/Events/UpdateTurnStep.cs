namespace truename.Events;

public class UpdateTurnStep : IGameEvent
{
    public string NextStep { get; set; }

    public UpdateTurnStep(string nextStep)
    {
        NextStep = nextStep;
    }

    public GameEffect Resolve => (m, _, _, _) =>
    {
        m.CurrentStep = NextStep;
    };
}