using System.Collections.Generic;
using System.Linq;
using System.Windows;
using truename.Systems;

namespace truename.Wpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private Player[] players;
    private Game match;
    private IEnumerable<GameEvent> game;

    public MainWindow()
    {
      InitializeComponent();

      players = new[]
      {
        new Player("tron_travolta", TestData.GrixisDeck),
        new Player("typedef", TestData.ReanimatorDeck),
      };

      var gameNum = 1;
      match = new Game(gameNum, players);
      var ruleSystem = new RuleSystem(match);
      game = ruleSystem.PlayGame();
      
      var events = new List<GameEvent>();

      foreach (var @event in game)
      {
        if (@event.Choices.Any())
          @event.Choices.First().Action();
        events.Add(@event);
      }
      
      DataContext = match;
    }
  }
}
