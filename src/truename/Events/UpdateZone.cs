namespace truename.Events;

public record UpdateZone(
  (Zones, Guid?) ZoneId,
  IEnumerable<Card> Cards
);
