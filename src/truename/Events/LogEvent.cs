namespace truename.Events;

public record LogEvent((string, int, string) Key, IGameEvent GameEvent);