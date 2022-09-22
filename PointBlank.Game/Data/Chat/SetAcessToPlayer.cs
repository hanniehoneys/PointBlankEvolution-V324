using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Enums;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using System;

namespace PointBlank.Game.Data.Chat
{
  public static class SetAcessToPlayer
  {
    public static string SetAcessPlayer(string str)
    {
      string[] strArray = str.Substring(str.IndexOf(" ") + 1).Split(' ');
      long int64 = Convert.ToInt64(strArray[0]);
      int int32 = Convert.ToInt32(strArray[1]);
      Account account = AccountManager.getAccount(int64, 0);
      if (account == null)
        return Translation.GetLabel("[*]SetAcess_Fail4");
      switch (int32)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
          if (!PlayerManager.updateAccountAccess(account.player_id, int32))
            return Translation.GetLabel("SetAcessF");
          try
          {
            account.access = (AccessLevel) int32;
            return Translation.GetLabel("SetAcessS", (object) int32, (object) account.player_name);
          }
          catch
          {
            return Translation.GetLabel("SetAcessF");
          }
        default:
          return Translation.GetLabel("[*]SetAcess_Fail4");
      }
    }
  }
}
