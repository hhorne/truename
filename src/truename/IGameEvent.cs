namespace truename;

public interface IGameEvent
{
  string Name { get; }
  void Resolve(Game g);
}
