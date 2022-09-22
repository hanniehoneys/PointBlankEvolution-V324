using PointBlank.Core;
using PointBlank.Core.Managers.Server;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Data.Chat
{
  public static class EnableMissions
  {
    public static string genCode1(string str, Account player)
    {
      bool mission = bool.Parse(str.Substring(8));
      if (!ServerConfigSyncer.updateMission(GameManager.Config, mission))
        return Translation.GetLabel("ActivateMissionsMsg2");
      Logger.warning(Translation.GetLabel("ActivateMissionsWarn", (object) mission, (object) player.player_name));
      return Translation.GetLabel("ActivateMissionsMsg1");
    }
  }
}
