using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PointBlank.Game.Data.Chat
{
  public static class GetAccountInfo
  {
    public static string getById(string str, Account player)
    {
      return GetAccountInfo.BaseCode(AccountManager.getAccount(long.Parse(str.Substring(5)), 2), player);
    }

    public static string getByNick(string str, Account player)
    {
      return GetAccountInfo.BaseCode(AccountManager.getAccount(str.Substring(5), 1, 2), player);
    }

    private static string BaseCode(Account p, Account player)
    {
      if (p == null || player == null)
        return Translation.GetLabel("GI_Fail");
      DateTime dateTime = p.LastLoginDate != 0U ? DateTime.ParseExact(p.LastLoginDate.ToString(), "yyMMddHHmm", (IFormatProvider) CultureInfo.InvariantCulture) : new DateTime();
      string str1 = Translation.GetLabel("GI_Title") + "\n" + Translation.GetLabel("GI_Id", (object) p.player_id) + "\n" + Translation.GetLabel("GI_Nick", (object) p.player_name) + "\n" + Translation.GetLabel("GI_Rank", (object) p._rank) + "\n" + Translation.GetLabel("GI_Fights", (object) p._statistic.fights, (object) p._statistic.fights_win, (object) p._statistic.fights_lost, (object) p._statistic.fights_draw) + "\n" + Translation.GetLabel("GI_KD", (object) p._statistic.GetKDRatio()) + "\n" + Translation.GetLabel("GI_HS", (object) p._statistic.GetHSRatio()) + "\n" + Translation.GetLabel("GI_LastLogin", dateTime == new DateTime() ? (object) "Nunca" : (object) dateTime.ToString("dd/MM/yy HH:mm")) + "\n" + Translation.GetLabel("GI_LastIP", player.access >= AccessLevel.Admin ? (object) p.PublicIP.ToString() : (object) Translation.GetLabel("GI_BlockedInfo")) + "\n" + Translation.GetLabel("GI_BanObj", (object) player.ban_obj_id);
      string str2;
      if (player.access >= AccessLevel.Admin)
        str2 = str1 + "\n" + Translation.GetLabel("GI_HaveAccess2", (object) p.access);
      else
        str2 = str1 + "\n" + Translation.GetLabel("GI_HaveAccess1", p.HaveGMLevel() ? (object) Translation.GetLabel("GI_Yes") : (object) Translation.GetLabel("GI_No"));
      string msg = str2 + "\n" + Translation.GetLabel("GI_UsingFake", p._bonus == null || p._bonus.fakeRank == 55 ? (object) Translation.GetLabel("GI_No") : (object) Translation.GetLabel("GI_Yes")) + "\n" + Translation.GetLabel("GI_Channel", p.channelId >= 0 ? (object) string.Format("{0:0##}", (object) (p.channelId + 1)) : (object) Translation.GetLabel("GI_None1")) + "\n" + Translation.GetLabel("GI_Room", p._room != null ? (object) string.Format("{0:0##}", (object) (p._room._roomId + 1)) : (object) Translation.GetLabel("GI_None2"));
      player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg));
      return Translation.GetLabel("GI_Success");
    }

    public static string getByIPAddress(string str, Account player)
    {
      List<string> accountsByIp = AccountManager.getAccountsByIP(str.Substring(6));
      string msg = Translation.GetLabel("AccIP_Title");
      foreach (string str1 in accountsByIp)
        msg = msg + "\n" + str1;
      player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg));
      return Translation.GetLabel("AccIP_Result", (object) accountsByIp.Count);
    }
  }
}
