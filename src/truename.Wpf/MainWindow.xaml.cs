using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace truename.Wpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private Player[] players;
    private Game match;
    private IEnumerable<IEvent> game;
    private ObservableCollection<IEvent> gameEvents;

    public MainWindow()
    {
      InitializeComponent();

      players = new[]
      {
        new Player("tron_travolta", TestData.GrixisDeck),
        new Player("typedef", TestData.ReanimatorDeck),
      };

      match = new Game(players);
      //ruleSystem = new RuleSystem(match);
      //game = ruleSystem.PlayGame();

      //gameEvents = new ObservableCollection<IEvent>(match.EventLog);
      //GameLog.DataContext = gameEvents;

      //foreach (var @event in game)
      //{
      //  if (@event is Decision)
      //  {
      //    var choice = (Decision)@event;
      //    choice.Choices.First().Action();
      //  }

      //  gameEvents.Add(@event);
      //}
    }
  }
}
