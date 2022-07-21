namespace truename.Events;

public class PlayerJoined : IGameEvent
{
    public Player Player { get; set; }

    public PlayerJoined(Player player)
    {
        Player = player;
    }

    public GameEffect Resolve => (m, _, _, _) =>
    {
        if (m.CurrentStep != Turn.PreGame.WaitingForPlayers)
        {
            throw new Exception("Player attempted to join a match already in progress.");
        }

        if (m.Players.Count == m.PlayerCount)
        {
            throw new Exception("Player attempted to join match that was full.");
        }

        m.Players.Add(Player.Id, Player);

        if (m.Players.Count == m.PlayerCount)
        {
            m.Apply(new UpdateTurnStep(Turn.PreGame.DetermineTurnOrder));
        }
    };
}