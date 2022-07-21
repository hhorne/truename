namespace truename.Events;

public class Skip<T> : IGameEvent where T : IGameEvent
{
    public T SkippedEvent { get; set; }
 
    public Skip(T @event)
    {
        SkippedEvent = @event;
    }
    public GameEffect Resolve => (_,_,_,_) => { };
}
