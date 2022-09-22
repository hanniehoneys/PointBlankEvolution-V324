using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Game.Data.Configs;

namespace PointBlank.Game.Data.Chat
{
  public static class ChangeUdpType
  {
    public static string SetUdpType(string str)
    {
      int num = int.Parse(str.Substring(4));
      if ((UdpState) num == GameConfig.udpType)
        return Translation.GetLabel("ChangeUDPAlready");
      if (num < 1 || num > 4)
        return Translation.GetLabel("ChangeUDPWrongValue");
      GameConfig.udpType = (UdpState) num;
      return Translation.GetLabel("ChangeUDPSuccess", (object) GameConfig.udpType);
    }
  }
}
