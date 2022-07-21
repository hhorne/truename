using truename.Events;

namespace truename.Abilities.Common;

public class LeylinePlayFreeAbility : ActivatedAbility
{
    public LeylinePlayFreeAbility(string label, string text)
        : base(label, text)
    {
        Conditions = new GameCondition[]
        {
            (m, _, _, _) => m.CurrentStep == Turn.PreGame.PreGameActions,
            (m, sourceId, _, ownerId) => m
                .Zones[(ZoneTypes.Hand, ownerId)]
                .Any(c => c.ZonedId == sourceId),
        };

        Effect = (m, srcId, controllerId, _) =>
            m.Apply(new PutIntoPlay
            (
                controllerId,
                srcId,
                (ZoneTypes.Hand, controllerId)
            ));
    }
}