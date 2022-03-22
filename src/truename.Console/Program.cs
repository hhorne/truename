using Spectre.Console;
using System.Linq;
using truename;

var players = new[]
{
  new Player("tron_travolta", TestData.GrixisDeck),
  new Player("typedef", TestData.ReanimatorDeck),
};

var match = new Game(players);
//var ruleSystem = new RuleSystem(match);

//foreach (var @event in ruleSystem.PlayGame())
//{
//  if (@event is Decision decision)
//  {
//    AnsiConsole
//      .Prompt(decision.ToPrompt())
//      .Action();
//  }
//  else
//  {
//    AnsiConsole.MarkupLine(@event.Name);
//    if (!string.IsNullOrEmpty(@event.Description))
//      AnsiConsole.MarkupLine(@event.Description);
//  }
//}
