using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private RuleSystem ruleSystem;
    private IEnumerable<GameEvent> game;
    private ObservableCollection<GameEvent> gameEvents;

    public MainWindow()
    {
      InitializeComponent();

      players = new[]
      {
        new Player("tron_travolta", TestData.GrixisDeck),
        new Player("typedef", TestData.ReanimatorDeck),
      };

      match = new Game(players);
      ruleSystem = new RuleSystem(match);
      game = ruleSystem.PlayGame();

      gameEvents = new ObservableCollection<GameEvent>(match.EventLog);
      GameLog.DataContext = gameEvents;

      foreach (var @event in game)
      {
        if (@event.Choices.Any())
          @event.Choices.First().Action();
        gameEvents.Add(@event);
      }
    }
  }
}
