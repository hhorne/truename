namespace truename
{
  public static class TestData
  {
    public static Card[] CreateCards(string name, int count) =>
        Enumerable
            .Range(1, count)
            .Select(n => new Card(name))
            .ToArray();

    public static Card[] AndCards(this Card[] cards, string name, int count) =>
        cards
        .Concat(TestData.CreateCards(name, count))
        .ToArray();

    public static Card[] GrixisDeck = CreateCards("thoughtseize", 4)
        .AndCards("snapcaster-mage", 4)
        .AndCards("fatal-push", 4)
        .AndCards("lightning-bolt", 4)
        .AndCards("brainstorm", 4)
        .AndCards("murktide-regent", 2)
        .AndCards("underground-sea", 3)
        .AndCards("volcanic-island", 3)
        .AndCards("scalding-tarn", 2)
        .AndCards("polluted-delta", 2)
        .AndCards("badlands", 2)
        .AndCards("island", 2)
        .AndCards("swamp", 2)
        .AndCards("mountain", 2);

    public static Card[] ReanimatorDeck = CreateCards("griselbrand", 4)
      .AndCards("chancellor-of-the-annex", 4)
      .AndCards("dark-ritual", 4)
      .AndCards("entomb", 4)
      .AndCards("reanimate", 4)
      .AndCards("faithless-looting", 4)
      .AndCards("badlands", 4)
      .AndCards("basic-mountain", 2)
      .AndCards("basic-swamp", 6)
      .AndCards("bloodstained-mire", 4);
  }
}