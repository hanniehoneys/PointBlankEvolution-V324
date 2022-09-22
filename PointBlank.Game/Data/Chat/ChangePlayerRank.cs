using PointBlank.Core;
using PointBlank.Core.Models.Account.Rank;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Data.Chat
{
  public static class ChangePlayerRank
  {
    public static string SetPlayerRank(string str)
    {
      string[] strArray = str.Substring(str.IndexOf(" ") + 1).Split(' ');
      long int64 = Convert.ToInt64(strArray[0]);
      int int32 = Convert.ToInt32(strArray[1]);
      if (int32 > 60 || int32 == 56 || (int32 < 0 || int64 <= 0L))
        return Translation.GetLabel("ChangePlyRankWrongValue");
      PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(int64, 0);
      if (account == null)
        return Translation.GetLabel("ChangePlyRankFailPlayer");
      if (!ComDiv.updateDB("accounts", "rank", (object) int32, "player_id", (object) account.player_id))
        return Translation.GetLabel("ChangePlyRankFailUnk");
      RankModel rank = RankXml.getRank(int32);
      account._rank = int32;
      SendItemInfo.LoadGoldCash(account);
      account.SendPacket((SendPacket) new PROTOCOL_BASE_RANK_UP_ACK(account._rank, rank != null ? rank._onNextLevel : 0), false);
      if (account._room != null)
        account._room.updateSlotsInfo();
      return Translation.GetLabel("ChangePlyRankSuccess", (object) int32);
    }
  }
}
