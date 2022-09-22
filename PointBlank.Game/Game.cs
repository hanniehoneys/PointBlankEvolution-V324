using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PointBlank.Game
{
  public static class Game
  {
    public static async void Update()
    {
      while (true)
      {
        Console.Title = "Point Blank - Game [Users: " + (object) GameManager._socketList.Count + " Online: " + (object) ServersXml.getServer(GameConfig.serverId)._LastCount + " Used RAM: " + (object) (GC.GetTotalMemory(true) / 1024L) + " KB]";
        ComDiv.updateDB("onlines", "game", (object) ServersXml.getServer(GameConfig.serverId)._LastCount);
        if (DateTime.Now.ToString("HH:mm") == "00:00")
        {
          foreach (PointBlank.Game.Data.Model.Account account in (IEnumerable<PointBlank.Game.Data.Model.Account>) AccountManager._accounts.Values)
          {
            if (account != null)
              account.Daily = new PlayerDailyRecord();
          }
          foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
          {
            if (gameClient != null && gameClient._player != null && gameClient._player._isOnline)
              gameClient._player.Daily = new PlayerDailyRecord();
          }
          ComDiv.updateDB("player_dailyrecord", "total", (object) 0);
          ComDiv.updateDB("player_dailyrecord", "wins", (object) 0);
          ComDiv.updateDB("player_dailyrecord", "loses", (object) 0);
          ComDiv.updateDB("player_dailyrecord", "draws", (object) 0);
          ComDiv.updateDB("player_dailyrecord", "kills", (object) 0);
          ComDiv.updateDB("player_dailyrecord", "deaths", (object) 0);
          ComDiv.updateDB("player_dailyrecord", "headshots", (object) 0);
          ComDiv.updateDB("player_dailyrecord", "point", (object) 0);
          ComDiv.updateDB("player_dailyrecord", "exp", (object) 0);
        }
        await Task.Delay(1000);
      }
    }
  }
}
