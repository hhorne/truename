namespace truename.Events;

public record GameCreated(
  Guid Id,
  int Number,
  Player[] Players
);