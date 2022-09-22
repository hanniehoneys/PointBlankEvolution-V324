using Npgsql;
using PointBlank.Core;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Game.Data.Managers
{
  public static class AccountManager
  {
    public static SortedList<long, PointBlank.Game.Data.Model.Account> _accounts = new SortedList<long, PointBlank.Game.Data.Model.Account>();

    public static void AddAccount(PointBlank.Game.Data.Model.Account acc)
    {
      lock (AccountManager._accounts)
      {
        if (AccountManager._accounts.ContainsKey(acc.player_id))
          return;
        AccountManager._accounts.Add(acc.player_id, acc);
      }
    }

    public static List<string> getAccountsByIP(string ip)
    {
      List<string> stringList = new List<string>();
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@ip", (object) ip);
          command.CommandText = "SELECT player_name FROM accounts WHERE lastip=@ip";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
            stringList.Add(npgsqlDataReader.GetString(0));
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error("was a problem loading your account through Ip\r\n" + ex.ToString());
      }
      return stringList;
    }

    public static PointBlank.Game.Data.Model.Account getAccountDB(object valor, int type)
    {
      return AccountManager.getAccountDB(valor, type, 35);
    }

    public static PointBlank.Game.Data.Model.Account getAccountDB(
      object valor,
      int type,
      int searchDBFlag)
    {
      if (type == 2 && (long) valor == 0L || (type == 0 || type == 1) && (string) valor == "")
        return (PointBlank.Game.Data.Model.Account) null;
      PointBlank.Game.Data.Model.Account acc = (PointBlank.Game.Data.Model.Account) null;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@value", valor);
          NpgsqlCommand npgsqlCommand = command;
          string str1;
          switch (type)
          {
            case 0:
              str1 = "token";
              break;
            case 1:
              str1 = "player_name";
              break;
            default:
              str1 = "player_id";
              break;
          }
          string str2 = "SELECT * FROM accounts WHERE " + str1 + "=@value";
          npgsqlCommand.CommandText = str2;
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            acc = new PointBlank.Game.Data.Model.Account();
            acc.login = npgsqlDataReader.GetString(0);
            acc.password = npgsqlDataReader.GetString(1);
            acc.SetPlayerId(npgsqlDataReader.GetInt64(2), searchDBFlag);
            acc.player_name = npgsqlDataReader.GetString(3);
            acc.name_color = npgsqlDataReader.GetInt32(4);
            acc.clanId = npgsqlDataReader.GetInt32(5);
            acc._rank = npgsqlDataReader.GetInt32(6);
            acc._gp = npgsqlDataReader.GetInt32(7);
            acc._exp = npgsqlDataReader.GetInt32(8);
            acc.pc_cafe = npgsqlDataReader.GetInt32(9);
            acc._statistic.fights = npgsqlDataReader.GetInt32(10);
            acc._statistic.fights_win = npgsqlDataReader.GetInt32(11);
            acc._statistic.fights_lost = npgsqlDataReader.GetInt32(12);
            acc._statistic.kills_count = npgsqlDataReader.GetInt32(13);
            acc._statistic.deaths_count = npgsqlDataReader.GetInt32(14);
            acc._statistic.headshots_count = npgsqlDataReader.GetInt32(15);
            acc._statistic.escapes = npgsqlDataReader.GetInt32(16);
            acc.access = (AccessLevel) npgsqlDataReader.GetInt32(17);
            acc.SetPublicIP(npgsqlDataReader.GetString(18));
            acc.LastRankUpDate = (uint) npgsqlDataReader.GetInt64(20);
            acc._money = npgsqlDataReader.GetInt32(21);
            acc._isOnline = npgsqlDataReader.GetBoolean(22);
            acc._equip._primary = npgsqlDataReader.GetInt32(23);
            acc._equip._secondary = npgsqlDataReader.GetInt32(24);
            acc._equip._melee = npgsqlDataReader.GetInt32(25);
            acc._equip._grenade = npgsqlDataReader.GetInt32(26);
            acc._equip._special = npgsqlDataReader.GetInt32(27);
            acc._equip._red = npgsqlDataReader.GetInt32(28);
            acc._equip._blue = npgsqlDataReader.GetInt32(29);
            acc._equip._helmet = npgsqlDataReader.GetInt32(30);
            acc._equip._dino = npgsqlDataReader.GetInt32(31);
            acc._equip._beret = npgsqlDataReader.GetInt32(32);
            acc.brooch = npgsqlDataReader.GetInt32(33);
            acc.insignia = npgsqlDataReader.GetInt32(34);
            acc.medal = npgsqlDataReader.GetInt32(35);
            acc.blue_order = npgsqlDataReader.GetInt32(36);
            acc._mission.mission1 = npgsqlDataReader.GetInt32(37);
            acc.clanAccess = npgsqlDataReader.GetInt32(38);
            acc.clanDate = npgsqlDataReader.GetInt32(39);
            acc.effects = (CouponEffects) npgsqlDataReader.GetInt64(40);
            acc._statistic.fights_draw = npgsqlDataReader.GetInt32(41);
            acc._mission.mission2 = npgsqlDataReader.GetInt32(42);
            acc._mission.mission3 = npgsqlDataReader.GetInt32(43);
            acc._statistic.totalkills_count = npgsqlDataReader.GetInt32(44);
            acc._statistic.totalfights_count = npgsqlDataReader.GetInt32(45);
            acc._status.SetData((uint) npgsqlDataReader.GetInt64(46), acc.player_id);
            acc.LastLoginDate = (uint) npgsqlDataReader.GetInt64(47);
            acc._statistic.ClanGames = npgsqlDataReader.GetInt32(48);
            acc._statistic.ClanWins = npgsqlDataReader.GetInt32(49);
            acc.ban_obj_id = npgsqlDataReader.GetInt64(51);
            acc.token = npgsqlDataReader.GetString(52);
            acc.hwid = npgsqlDataReader.GetString(53);
            acc.age = npgsqlDataReader.GetInt32(55);
            acc.tourneyLevel = npgsqlDataReader.GetInt32(56);
            acc._statistic.assist = npgsqlDataReader.GetInt32(57);
            acc._equip.face = npgsqlDataReader.GetInt32(58);
            acc._equip.jacket = npgsqlDataReader.GetInt32(59);
            acc._equip.poket = npgsqlDataReader.GetInt32(60);
            acc._equip.glove = npgsqlDataReader.GetInt32(61);
            acc._equip.belt = npgsqlDataReader.GetInt32(62);
            acc._equip.holster = npgsqlDataReader.GetInt32(63);
            acc._equip.skin = npgsqlDataReader.GetInt32(64);
            AccountManager.AddAccount(acc);
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error("was a problem loading the accounts\r\n" + ex.ToString());
      }
      return acc;
    }

    public static void getFriendlyAccounts(FriendSystem system)
    {
      if (system == null)
        return;
      if (system._friends.Count == 0)
        return;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          List<string> stringList = new List<string>();
          for (int index = 0; index < system._friends.Count; ++index)
          {
            Friend friend = system._friends[index];
            string parameterName = "@valor" + (object) index;
            command.Parameters.AddWithValue(parameterName, (object) friend.player_id);
            stringList.Add(parameterName);
          }
          string str = string.Join(",", stringList.ToArray());
          command.CommandText = "SELECT player_name,player_id,rank,online,status FROM accounts WHERE player_id in (" + str + ") ORDER BY player_id";
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            Friend friend = system.GetFriend(npgsqlDataReader.GetInt64(1));
            if (friend != null)
            {
              friend.player.player_name = npgsqlDataReader.GetString(0);
              friend.player._rank = npgsqlDataReader.GetInt32(2);
              friend.player._isOnline = npgsqlDataReader.GetBoolean(3);
              friend.player._status.SetData((uint) npgsqlDataReader.GetInt64(4), friend.player_id);
            }
          }
          command.Dispose();
          npgsqlDataReader.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error("was a problem loading (FriendlyAccounts)!\r\n" + ex.ToString());
      }
    }

    public static PointBlank.Game.Data.Model.Account getAccount(long id, int searchFlag)
    {
      if (id == 0L)
        return (PointBlank.Game.Data.Model.Account) null;
      try
      {
        PointBlank.Game.Data.Model.Account account = (PointBlank.Game.Data.Model.Account) null;
        return AccountManager._accounts.TryGetValue(id, out account) ? account : AccountManager.getAccountDB((object) id, 2, searchFlag);
      }
      catch
      {
        return (PointBlank.Game.Data.Model.Account) null;
      }
    }

    public static PointBlank.Game.Data.Model.Account getAccount(long id, bool noUseDB)
    {
      if (id == 0L)
        return (PointBlank.Game.Data.Model.Account) null;
      try
      {
        PointBlank.Game.Data.Model.Account account = (PointBlank.Game.Data.Model.Account) null;
        return AccountManager._accounts.TryGetValue(id, out account) ? account : (noUseDB ? (PointBlank.Game.Data.Model.Account) null : AccountManager.getAccountDB((object) id, 2));
      }
      catch
      {
        return (PointBlank.Game.Data.Model.Account) null;
      }
    }

    public static PointBlank.Game.Data.Model.Account getAccount(
      string text,
      int type,
      int searchFlag)
    {
      if (string.IsNullOrEmpty(text))
        return (PointBlank.Game.Data.Model.Account) null;
      IList<PointBlank.Game.Data.Model.Account> values = AccountManager._accounts.Values;
      for (int index = 0; index < values.Count; ++index)
      {
        PointBlank.Game.Data.Model.Account account = values[index];
        if (account != null && (type == 1 && account.player_name == text && account.player_name.Length > 0 || type == 0 && string.Compare(account.login, text) == 0))
          return account;
      }
      return AccountManager.getAccountDB((object) text, type, searchFlag);
    }

    public static bool updatePlayerName(string name, long playerId)
    {
      return ComDiv.updateDB("accounts", "player_name", (object) name, "player_id", (object) playerId);
    }
  }
}
