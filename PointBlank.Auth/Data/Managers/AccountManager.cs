using Npgsql;
using PointBlank.Core;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace PointBlank.Auth.Data.Managers
{
  public class AccountManager
  {
    public SortedList<long, PointBlank.Auth.Data.Model.Account> _accounts = new SortedList<long, PointBlank.Auth.Data.Model.Account>();
    private static AccountManager acm = new AccountManager();

    public bool AddAccount(PointBlank.Auth.Data.Model.Account acc)
    {
      lock (this._accounts)
      {
        if (!this._accounts.ContainsKey(acc.player_id))
        {
          this._accounts.Add(acc.player_id, acc);
          return true;
        }
      }
      return false;
    }

    public PointBlank.Auth.Data.Model.Account getAccountDB(
      object valor,
      object valor2,
      int type,
      int searchFlag)
    {
      if (type == 0 && (string) valor == "" || type == 1 && (long) valor == 0L || type == 2 && (string.IsNullOrEmpty((string) valor) || string.IsNullOrEmpty((string) valor2)))
        return (PointBlank.Auth.Data.Model.Account) null;
      PointBlank.Auth.Data.Model.Account acc = (PointBlank.Auth.Data.Model.Account) null;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@valor", valor);
          switch (type)
          {
            case 0:
              command.CommandText = "SELECT * FROM accounts WHERE token=@valor LIMIT 1";
              break;
            case 1:
              command.CommandText = "SELECT * FROM accounts WHERE player_id=@valor LIMIT 1";
              break;
            case 2:
              command.Parameters.AddWithValue("@valor2", valor2);
              command.CommandText = "SELECT * FROM accounts WHERE login=@valor AND password=@valor2 LIMIT 1";
              break;
          }
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            acc = new PointBlank.Auth.Data.Model.Account();
            acc.login = npgsqlDataReader.GetString(0);
            acc.password = npgsqlDataReader.GetString(1);
            acc.SetPlayerId(npgsqlDataReader.GetInt64(2), searchFlag);
            acc.player_name = npgsqlDataReader.GetString(3);
            acc.name_color = npgsqlDataReader.GetInt32(4);
            acc.clan_id = npgsqlDataReader.GetInt32(5);
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
            acc.access = npgsqlDataReader.GetInt32(17);
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
            acc.effects = (CouponEffects) npgsqlDataReader.GetInt64(40);
            acc._statistic.fights_draw = npgsqlDataReader.GetInt32(41);
            acc._mission.mission2 = npgsqlDataReader.GetInt32(42);
            acc._mission.mission3 = npgsqlDataReader.GetInt32(43);
            acc._statistic.totalkills_count = npgsqlDataReader.GetInt32(44);
            acc._statistic.totalfights_count = npgsqlDataReader.GetInt32(45);
            acc._status.SetData((uint) npgsqlDataReader.GetInt64(46), acc.player_id);
            acc.MacAddress = (PhysicalAddress) npgsqlDataReader.GetValue(50);
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
            if (this.AddAccount(acc) && acc._isOnline)
              acc.setOnlineStatus(false);
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
        Logger.error("was a problem loading accounts!\r\n" + ex.ToString());
      }
      return acc;
    }

    public void getFriendlyAccounts(FriendSystem system)
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
          command.CommandText = "SELECT player_name, player_id, rank, online, status FROM accounts WHERE player_id in (" + str + ") ORDER BY player_id";
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
              if (friend.player._isOnline && !this._accounts.ContainsKey(friend.player_id))
              {
                friend.player.setOnlineStatus(false);
                friend.player._status.ResetData(friend.player_id);
              }
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

    public void getFriendlyAccounts(FriendSystem system, bool isOnline)
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
          List<string> stringList = new List<string>();
          for (int index = 0; index < system._friends.Count; ++index)
          {
            Friend friend = system._friends[index];
            if (friend.state > 0)
              return;
            string parameterName = "@valor" + (object) index;
            command.Parameters.AddWithValue(parameterName, (object) friend.player_id);
            stringList.Add(parameterName);
          }
          string str = string.Join(",", stringList.ToArray());
          if (str == "")
            return;
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@on", (object) isOnline);
          command.CommandText = "SELECT player_name, player_id, rank, status FROM accounts WHERE player_id in (" + str + ") AND online=@on ORDER BY player_id";
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            Friend friend = system.GetFriend(npgsqlDataReader.GetInt64(1));
            if (friend != null)
            {
              friend.player.player_name = npgsqlDataReader.GetString(0);
              friend.player._rank = npgsqlDataReader.GetInt32(2);
              friend.player._isOnline = isOnline;
              friend.player._status.SetData((uint) npgsqlDataReader.GetInt64(3), friend.player_id);
              if (isOnline && !this._accounts.ContainsKey(friend.player_id))
              {
                friend.player.setOnlineStatus(false);
                friend.player._status.ResetData(friend.player_id);
              }
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
        Logger.error("was a problem loading (FriendAccounts2)!\r\n" + ex.ToString());
      }
    }

    public static AccountManager getInstance()
    {
      return AccountManager.acm;
    }

    public PointBlank.Auth.Data.Model.Account getAccount(long id)
    {
      if (id == 0L)
        return (PointBlank.Auth.Data.Model.Account) null;
      try
      {
        PointBlank.Auth.Data.Model.Account account = (PointBlank.Auth.Data.Model.Account) null;
        return this._accounts.TryGetValue(id, out account) ? account : this.getAccountDB((object) id, (object) null, 1, 0);
      }
      catch
      {
        return (PointBlank.Auth.Data.Model.Account) null;
      }
    }

    public PointBlank.Auth.Data.Model.Account getAccount(long id, bool noUseDB)
    {
      if (id == 0L)
        return (PointBlank.Auth.Data.Model.Account) null;
      try
      {
        PointBlank.Auth.Data.Model.Account account = (PointBlank.Auth.Data.Model.Account) null;
        return this._accounts.TryGetValue(id, out account) ? account : (noUseDB ? (PointBlank.Auth.Data.Model.Account) null : this.getAccountDB((object) id, (object) null, 1, 0));
      }
      catch
      {
        return (PointBlank.Auth.Data.Model.Account) null;
      }
    }

    public bool CreateAccount(out PointBlank.Auth.Data.Model.Account p, string login, string password)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@login", (object) login);
          command.Parameters.AddWithValue("@pass", (object) password);
          command.CommandText = "INSERT INTO accounts (login, password) VALUES (@login, @pass)";
          command.ExecuteNonQuery();
          command.CommandText = "SELECT * FROM accounts WHERE login=@login";
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          PointBlank.Auth.Data.Model.Account acc = new PointBlank.Auth.Data.Model.Account();
          while (npgsqlDataReader.Read())
          {
            acc.login = login;
            acc.password = password;
            acc.player_id = npgsqlDataReader.GetInt64(2);
            acc.SetPlayerId();
            acc.player_name = npgsqlDataReader.GetString(3);
            acc.name_color = npgsqlDataReader.GetInt32(4);
            acc.clan_id = npgsqlDataReader.GetInt32(5);
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
            acc.access = npgsqlDataReader.GetInt32(17);
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
            acc.effects = (CouponEffects) npgsqlDataReader.GetInt64(40);
            acc._statistic.fights_draw = npgsqlDataReader.GetInt32(41);
            acc._mission.mission2 = npgsqlDataReader.GetInt32(42);
            acc._mission.mission3 = npgsqlDataReader.GetInt32(43);
            acc._statistic.totalkills_count = npgsqlDataReader.GetInt32(44);
            acc._statistic.totalfights_count = npgsqlDataReader.GetInt32(45);
            acc._status.SetData((uint) npgsqlDataReader.GetInt64(46), acc.player_id);
            acc.MacAddress = (PhysicalAddress) npgsqlDataReader.GetValue(50);
            acc.ban_obj_id = npgsqlDataReader.GetInt64(51);
            acc.age = npgsqlDataReader.GetInt32(55);
            acc.tourneyLevel = npgsqlDataReader.GetInt32(56);
          }
          p = acc;
          this.AddAccount(acc);
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
          return true;
        }
      }
      catch (Exception ex)
      {
        Logger.warning("[AccountManager.CreateAccount] " + ex.ToString());
        p = (PointBlank.Auth.Data.Model.Account) null;
        return false;
      }
    }

    public bool CreateAccount(out PointBlank.Auth.Data.Model.Account p, string Token)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@login", (object) "NONE USER");
          command.Parameters.AddWithValue("@pass", (object) "NONE PASSWORD");
          command.Parameters.AddWithValue("@token", (object) Token);
          command.CommandText = "INSERT INTO accounts (login, password, token) VALUES (@login, @pass, @token)";
          command.ExecuteNonQuery();
          command.CommandText = "SELECT * FROM accounts WHERE token=@token";
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          PointBlank.Auth.Data.Model.Account acc = new PointBlank.Auth.Data.Model.Account();
          while (npgsqlDataReader.Read())
          {
            acc.login = "NONE USER";
            acc.password = "NONE PASSWORD";
            acc.token = Token;
            acc.player_id = npgsqlDataReader.GetInt64(2);
            acc.SetPlayerId();
            acc.player_name = npgsqlDataReader.GetString(3);
            acc.name_color = npgsqlDataReader.GetInt32(4);
            acc.clan_id = npgsqlDataReader.GetInt32(5);
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
            acc.access = npgsqlDataReader.GetInt32(17);
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
            acc.effects = (CouponEffects) npgsqlDataReader.GetInt64(40);
            acc._statistic.fights_draw = npgsqlDataReader.GetInt32(41);
            acc._mission.mission2 = npgsqlDataReader.GetInt32(42);
            acc._mission.mission3 = npgsqlDataReader.GetInt32(43);
            acc._statistic.totalkills_count = npgsqlDataReader.GetInt32(44);
            acc._statistic.totalfights_count = npgsqlDataReader.GetInt32(45);
            acc._status.SetData((uint) npgsqlDataReader.GetInt64(46), acc.player_id);
            acc.MacAddress = (PhysicalAddress) npgsqlDataReader.GetValue(50);
            acc.ban_obj_id = npgsqlDataReader.GetInt64(51);
            acc.age = npgsqlDataReader.GetInt32(55);
            acc.tourneyLevel = npgsqlDataReader.GetInt32(56);
          }
          p = acc;
          this.AddAccount(acc);
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
          return true;
        }
      }
      catch (Exception ex)
      {
        Logger.warning("[AccountManager.CreateAccount] " + ex.ToString());
        p = (PointBlank.Auth.Data.Model.Account) null;
        return false;
      }
    }
  }
}
