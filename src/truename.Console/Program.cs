﻿using Spectre.Console;
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
  if (@event.Choices.Length <= 1)
  {
    AnsiConsole.WriteLine(@event.Name);
    if (!string.IsNullOrEmpty(@event.Description))
      AnsiConsole.WriteLine(@event.Description);
  }
  else
  {
    AnsiConsole
      .Prompt(@event.ToPrompt())
      .Action();
  }
}
