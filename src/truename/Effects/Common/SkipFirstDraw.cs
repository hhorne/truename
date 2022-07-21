using truename.Events;

namespace truename.Effects.Common;

public class SkipFirstDraw : ReplacementEffect<Draw>
{
    public SkipFirstDraw()
    {
        AppliesTo = IsExpired = (m, e) =>
        {
            var drawStep = m.CurrentStep == Turn.Steps.Draw;
            var firstPlayer = m.TurnOrder.First();
            var onThePlay = firstPlayer == m.ActivePlayerId;
            var firstDraw = m.EventLog
                .OfType<Draw>()
                .None(e => e.PlayerId == firstPlayer);

            return drawStep && onThePlay && firstDraw;
        };

        CreateReplacement = (m, e) => new Skip<Draw>(e);
    }
}
