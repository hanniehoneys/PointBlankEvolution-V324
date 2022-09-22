using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Data.Chat
{
  public static class Ban
  {
    public static string UpdateReason(string str)
    {
      string str1 = str.Substring(7);
      int num = str1.IndexOf(" ");
      if (num < 0)
        return "Invalid command. [Server]";
      if (BanManager.SaveBanReason(long.Parse(str1.Split(' ')[0]), str1.Substring(num + 1)))
        return Translation.GetLabel("PlayerBanReasonSuccess");
      return Translation.GetLabel("PlayerBanReasonFail");
    }

    public static string BanForeverNick(string str, Account player, bool warn)
    {
      Account account = AccountManager.getAccount(str.Substring(6), 1, 0);
      return Ban.BaseBanForever(player, account, warn);
    }

    public static string BanForeverId(string str, Account player, bool warn)
    {
      Account account = AccountManager.getAccount(long.Parse(str.Substring(7)), 0);
      return Ban.BaseBanForever(player, account, warn);
    }

    public static string BanNormalNick(string str, Account player, bool warn)
    {
      string[] strArray = str.Substring(5).Split(' ');
      string text = strArray[0];
      DateTime endDate = DateTime.Now.AddDays(Convert.ToDouble(strArray[1]));
      Account account = AccountManager.getAccount(text, 1, 0);
      return Ban.BaseBanNormal(player, account, warn, endDate);
    }

    public static string BanNormalId(string str, Account player, bool warn)
    {
      string[] strArray = str.Substring(6).Split(' ');
      long int64 = Convert.ToInt64(strArray[0]);
      DateTime endDate = DateTime.Now.AddDays(Convert.ToDouble(strArray[1]));
      Account account = AccountManager.getAccount(int64, 0);
      return Ban.BaseBanNormal(player, account, warn, endDate);
    }

    private static string BaseBanNormal(
      Account player,
      Account victim,
      bool warn,
      DateTime endDate)
    {
      if (victim == null)
        return Translation.GetLabel("PlayerBanUserInvalid");
      if (victim.access > player.access)
        return Translation.GetLabel("PlayerBanAccessInvalid");
      if (player.player_id == victim.player_id)
        return Translation.GetLabel("PlayerBanSimilarID");
      BanHistory banHistory = BanManager.SaveHistory(player.player_id, "DURATION", victim.player_id.ToString(), endDate);
      if (banHistory == null)
        return Translation.GetLabel("PlayerBanFail");
      if (warn)
      {
        using (PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK messageAnnounceAck = new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(Translation.GetLabel("PlayerBannedWarning2", (object) victim.player_name)))
          GameManager.SendPacketToAllClients((SendPacket) messageAnnounceAck);
      }
      victim.ban_obj_id = banHistory.object_id;
      victim.SendPacket((SendPacket) new PROTOCOL_AUTH_ACCOUNT_KICK_ACK(2), false);
      victim.Close(1000, true);
      return Translation.GetLabel("PlayerBanSuccess", (object) banHistory.object_id);
    }

    private static string BaseBanForever(Account player, Account victim, bool warn)
    {
      if (victim == null)
        return Translation.GetLabel("PlayerBanUserInvalid");
      if (victim.access > player.access)
        return Translation.GetLabel("PlayerBanAccessInvalid");
      if (player.player_id == victim.player_id)
        return Translation.GetLabel("PlayerBanSimilarID");
      if (!ComDiv.updateDB("accounts", "access_level", (object) -1, "player_id", (object) victim.player_id))
        return Translation.GetLabel("PlayerBanFail");
      if (warn)
      {
        using (PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK messageAnnounceAck = new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(Translation.GetLabel("PlayerBannedWarning", (object) victim.player_name)))
          GameManager.SendPacketToAllClients((SendPacket) messageAnnounceAck);
      }
      victim.access = AccessLevel.Banned;
      victim.SendPacket((SendPacket) new PROTOCOL_AUTH_ACCOUNT_KICK_ACK(2), false);
      victim.Close(1000, true);
      return Translation.GetLabel("PlayerBanSuccess", (object) -1);
    }

    public static string GetBanData(string str, Account player)
    {
      BanHistory accountBan = BanManager.GetAccountBan(long.Parse(str.Substring(7)));
      if (accountBan == null)
        return Translation.GetLabel("GetBanInfoError");
      string msg = Translation.GetLabel("GetBanInfoTitle") + "\n" + Translation.GetLabel("GetBanInfoProvider", (object) accountBan.provider_id) + "\n" + Translation.GetLabel("GetBanInfoType", (object) accountBan.type) + "\n" + Translation.GetLabel("GetBanInfoValue", (object) accountBan.value) + "\n" + Translation.GetLabel("GetBanInfoReason", (object) accountBan.reason) + "\n" + Translation.GetLabel("GetBanInfoStart", (object) accountBan.startDate) + "\n" + Translation.GetLabel("GetBanInfoEnd", (object) accountBan.endDate);
      player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg));
      return Translation.GetLabel("GetBanInfoSuccess");
    }
  }
}
