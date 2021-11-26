using Spectre.Console;

namespace truename;

public static class GameEventHelpers
{
  public static SelectionPrompt<GameAction> ToPrompt(this GameEvent @event)
  {
    var title = $"[red]{@event.Name}[/]";
    if (!string.IsNullOrWhiteSpace(@event.Description))
      title = $"{title}\n[italic white]{@event.Description}[/]";
    return new SelectionPrompt<GameAction>()
      .Title(title)
      .PageSize(@event.Actions.Length + 1)
      .AddChoices(@event.Actions);
  }
}