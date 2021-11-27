namespace truename.Effects.Predefined;

public class SkipFirstDraw : SkipDraw
{
  public SkipFirstDraw() : base((game, @event) =>
   {
     if (@event.Type == "Turn/Step/Draw")
     {
       var firstPlayer = game.TurnOrder.First();
       var playerId = @event.PlayerId;
       var turnNumber = game.Turns[playerId];
       return turnNumber == 1 && firstPlayer == playerId;
     }

     return false;
   })
  { }
}