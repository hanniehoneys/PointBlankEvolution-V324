using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Data.Chat
{
  public static class SendGoldToPlayerDev
  {
    public static string SendGoldToPlayer(string str)
    {
      string[] strArray = str.Substring(str.IndexOf(" ") + 1).Split(' ');
      long int64 = Convert.ToInt64(strArray[0]);
      int int32 = Convert.ToInt32(strArray[1]);
      Account account = AccountManager.getAccount(int64, 0);
      if (account == null || account._gp + int32 > 999999999)
        return Translation.GetLabel("[*]SendCash_Fail4");
      if (!PlayerManager.updateAccountCash(account.player_id, account._gp + int32))
        return Translation.GetLabel("GiveCashFail2");
      account._gp += int32;
      account.SendPacket((SendPacket) new PROTOCOL_AUTH_GET_POINT_CASH_ACK(0, account._gp, account._money), false);
      SendItemInfo.LoadGoldCash(account);
      return Translation.GetLabel("GiveCashSuccessD", (object) account._gp, (object) account.player_name);
    }
  }
}
