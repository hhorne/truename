using Spectre.Console;
using truename;
using truename.Systems;

var players = new[]
{
  new Player("tron_travolta", TestData.GrixisDeck),
  new Player("typedef", TestData.ReanimatorDeck),
};

var gameNum = 1;
var match = new Game(gameNum, players);
var ruleSystem = new RuleSystem(match);
var game = ruleSystem.PlayGame();

foreach (var @event in game)
{
  if (@event.Actions.Count() <= 1)
  {
    Console.WriteLine(@event.Name);
    if (!string.IsNullOrEmpty(@event.Description))
      Console.WriteLine(@event.Description);

    if (@event.Actions.Any())
    {
      @event
        .Actions
        .First()
        .Action();
    }
  }
  else
  {
    AnsiConsole
      .Prompt(@event.ToPrompt())
      .Action();
  }
}
