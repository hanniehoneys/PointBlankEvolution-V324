using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using System;

namespace PointBlank.Game.Data.Chat
{
  public static class UnBan
  {
    public static string UnbanByNick(string str, Account player)
    {
      Account account = AccountManager.getAccount(str.Substring(4), 1, 0);
      return UnBan.BaseUnbanNormal(player, account);
    }

    public static string UnbanById(string str, Account player)
    {
      Account account = AccountManager.getAccount(long.Parse(str.Substring(5)), 0);
      return UnBan.BaseUnbanNormal(player, account);
    }

    public static string SuperUnbanByNick(string str, Account player)
    {
      Account account = AccountManager.getAccount(str.Substring(5), 1, 0);
      return UnBan.BaseUnbanSuper(player, account);
    }

    public static string SuperUnbanById(string str, Account player)
    {
      Account account = AccountManager.getAccount(long.Parse(str.Substring(6)), 0);
      return UnBan.BaseUnbanSuper(player, account);
    }

    private static string BaseUnbanNormal(Account player, Account victim)
    {
      if (victim == null)
        return Translation.GetLabel("PlayerBanUserInvalid");
      if (victim.access == AccessLevel.Banned)
        return Translation.GetLabel("PlayerUnbanAccessInvalid");
      if (victim.ban_obj_id == 0L)
        return Translation.GetLabel("PlayerUnbanNoBan");
      if (victim.player_id == player.player_id)
        return Translation.GetLabel("PlayerUnbanSimilarId");
      if (ComDiv.updateDB("ban_history", "expire_date", (object) DateTime.Now, "object_id", (object) victim.ban_obj_id))
        return Translation.GetLabel("PlayerUnbanSuccess");
      return Translation.GetLabel("PlayerUnbanFail");
    }

    private static string BaseUnbanSuper(Account player, Account victim)
    {
      if (victim == null)
        return Translation.GetLabel("PlayerBanUserInvalid");
      if (victim.access != AccessLevel.Banned)
        return Translation.GetLabel("PlayerUnbanAccessInvalid");
      if (victim.player_id == player.player_id)
        return Translation.GetLabel("PlayerUnbanSimilarId");
      if (!ComDiv.updateDB("accounts", "access_level", (object) 0, "player_id", (object) victim.player_id))
        return Translation.GetLabel("PlayerUnbanFail");
      victim.access = AccessLevel.Normal;
      return Translation.GetLabel("PlayerUnbanSuccess");
    }
  }
}
