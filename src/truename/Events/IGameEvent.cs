namespace truename.Events;

public interface IGameEvent
{
    public GameEffect Resolve { get; }
}
