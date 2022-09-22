using PointBlank.Core;

namespace PointBlank.Game.Data.Chat
{
  public static class AFKInteraction
  {
    public static string GetAFKCount(string str)
    {
      return Translation.GetLabel("AFK_Count_Success", (object) GameManager.KickCountActiveClient(double.Parse(str.Substring(9))));
    }

    public static string KickAFKPlayers(string str)
    {
      return Translation.GetLabel("AFK_Kick_Success", (object) GameManager.KickActiveClient(double.Parse(str.Substring(8))));
    }
  }
}
