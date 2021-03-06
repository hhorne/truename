namespace truename.Effects.Predefined;

public class SkipFirstDraw : ReplacementEffect
{
  static EventConverter drawToSkip = (g, e) =>
    new GameEvent
    {
      Name = $"Skipping {e.Name}"
    };

  public SkipFirstDraw() : base((game, @event) =>
    {
      if (@event.Type == Turn.Draw)
      {
        var firstPlayer = game.TurnOrder.First();
        var playerId = @event.PlayerId;
        var turnNumber = game.Turns[playerId];
        return turnNumber == 1 && firstPlayer == playerId;
      }

      return false;
    }, drawToSkip)
  { }
}