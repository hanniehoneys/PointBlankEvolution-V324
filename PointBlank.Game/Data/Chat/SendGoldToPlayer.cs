using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Data.Chat
{
  public static class SendGoldToPlayer
  {
    public static string SendByNick(string str)
    {
      return SendGoldToPlayer.BaseGiveGold(AccountManager.getAccount(str.Substring(3), 1, 0));
    }

    public static string SendById(string str)
    {
      return SendGoldToPlayer.BaseGiveGold(AccountManager.getAccount(long.Parse(str.Substring(4)), 0));
    }

    private static string BaseGiveGold(Account pR)
    {
      if (pR == null)
        return Translation.GetLabel("GiveGoldFail");
      if (!PlayerManager.updateAccountGold(pR.player_id, pR._gp + 10000))
        return Translation.GetLabel("GiveGoldFail2");
      pR._gp += 10000;
      pR.SendPacket((SendPacket) new PROTOCOL_AUTH_GET_POINT_CASH_ACK(0, pR._gp, pR._money), false);
      SendItemInfo.LoadGoldCash(pR);
      return Translation.GetLabel("GiveGoldSuccess", (object) pR.player_name);
    }
  }
}
