using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace truename.Systems
{
  public abstract class System
  {
    protected Game game;

    public System(Game game)
    {
      this.game = game;
    }

    protected string GetPlayerName(string playerId) => game.Players[playerId].Name;
  }
}
