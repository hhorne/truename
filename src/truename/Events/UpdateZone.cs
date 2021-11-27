namespace truename.Events;

public record UpdateZone(
  (Zones, string) ZoneId,
  IEnumerable<Card> Cards
);
