using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using System;

namespace PointBlank.Game.Data.Chat
{
  public static class SetVipToPlayer
  {
    public static string SetVipPlayer(string str)
    {
      string[] strArray = str.Substring(str.IndexOf(" ") + 1).Split(' ');
      long int64 = Convert.ToInt64(strArray[0]);
      int int32 = Convert.ToInt32(strArray[1]);
      Account account = AccountManager.getAccount(int64, 0);
      if (account == null)
        return Translation.GetLabel("[*]SetVip_Fail4");
      switch (int32)
      {
        case 0:
        case 1:
        case 2:
          if (!PlayerManager.updateAccountVip(account.player_id, int32))
            return Translation.GetLabel("SetVipF");
          try
          {
            account.pc_cafe = int32;
            return Translation.GetLabel("SetVipS", (object) int32, (object) account.player_name);
          }
          catch
          {
            return Translation.GetLabel("SetVipF");
          }
        default:
          return Translation.GetLabel("[*]SetVip_Fail4");
      }
    }
  }
}
