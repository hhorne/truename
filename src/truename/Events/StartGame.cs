using truename.Effects;
using truename.Effects.Common;

namespace truename.Events;

public class StartGame : IGameEvent
{
    public int LifeTotal { get; }

    public StartGame(int lifeTotal)
    {
        LifeTotal = lifeTotal;
    }

    public GameEffect Resolve => (m, _, _, _) =>
    {
        if (m.Players.Count != m.PlayerCount)
        {
            throw new Exception("Can't start game until all players have joined.");
        }

        m.GameNumber++;

        m.Zones = new()
        {
            [(ZoneTypes.Battlefield, null)] = new(),
            [(ZoneTypes.Stack, null)] = new(),
            [(ZoneTypes.Exile, null)] = new()
        };

        m.Players.ForEach(p =>
        {
            m.LifeTotals[p.Key] = LifeTotal;
            m.Zones[(ZoneTypes.Graveyard, p.Key)] = new();
            m.Zones[(ZoneTypes.Hand, p.Key)] = new();
            m.Zones[(ZoneTypes.Library, p.Key)] = new(p.Value.DeckList);
            m.Apply(new ShuffleLibrary(p.Key));
        });

        m.ContinuousEffects.Add(new SkipFirstDraw());
    };
}
