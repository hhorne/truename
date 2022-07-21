namespace truename.Events;

public class SetTurnOrder : IGameEvent
{
    public string[] TurnOrder { get; }

    public SetTurnOrder(string[] turnOrder)
    {
        TurnOrder = turnOrder;
    }

    public GameEffect Resolve => (m, _, _, _) =>
    {
        if (m.CurrentStep != Turn.PreGame.DetermineTurnOrder)
        {
            throw new Exception("Turn Order can only be changed in the beginning of a game.");
        }

        var validPlayerList = Enumerable.SequenceEqual(
            m.Players.Select(p => p.Key).OrderBy(t => t),
            TurnOrder.OrderBy(t => t)
        );

        if (!validPlayerList)
        {
            throw new Exception("Players in turn order don't match players in match");
        }

        m.TurnOrder = TurnOrder;

        m.Apply(new UpdateTurnStep(Turn.PreGame.DrawOpeningHands));
    };
}
