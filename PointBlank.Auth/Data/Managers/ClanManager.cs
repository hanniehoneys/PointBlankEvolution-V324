using Npgsql;
using PointBlank.Core;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Auth.Data.Managers
{
  public class ClanManager
  {
    public static PointBlank.Core.Models.Account.Clan.Clan getClanDB(object valor, int type)
    {
      try
      {
        PointBlank.Core.Models.Account.Clan.Clan clan = new PointBlank.Core.Models.Account.Clan.Clan();
        if (type == 1 && (int) valor <= 0 || type == 0 && string.IsNullOrEmpty(valor.ToString()))
          return clan;
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          string str = type == 0 ? "clan_name" : "clan_id";
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@valor", valor);
          command.CommandText = "SELECT * FROM clan_data WHERE " + str + "=@valor";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            clan._id = npgsqlDataReader.GetInt32(0);
            clan._rank = npgsqlDataReader.GetInt32(1);
            clan._name = npgsqlDataReader.GetString(2);
            clan.owner_id = npgsqlDataReader.GetInt64(3);
            clan._logo = (uint) npgsqlDataReader.GetInt64(4);
            clan._name_color = npgsqlDataReader.GetInt32(5);
            clan.effect = npgsqlDataReader.GetInt32(24);
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
        return clan._id == 0 ? new PointBlank.Core.Models.Account.Clan.Clan() : clan;
      }
      catch
      {
        return new PointBlank.Core.Models.Account.Clan.Clan();
      }
    }

    public static List<PointBlank.Auth.Data.Model.Account> getClanPlayers(
      int clanId,
      long exception)
    {
      List<PointBlank.Auth.Data.Model.Account> accountList = new List<PointBlank.Auth.Data.Model.Account>();
      if (clanId <= 0)
        return accountList;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@clan", (object) clanId);
          command.CommandText = "SELECT player_id,player_name,rank,online,status FROM accounts WHERE clan_id=@clan";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            long int64 = npgsqlDataReader.GetInt64(0);
            if (int64 != exception)
            {
              PointBlank.Auth.Data.Model.Account account = new PointBlank.Auth.Data.Model.Account() { player_id = int64, player_name = npgsqlDataReader.GetString(1), _rank = npgsqlDataReader.GetInt32(2), _isOnline = npgsqlDataReader.GetBoolean(3) };
              account._status.SetData((uint) npgsqlDataReader.GetInt64(4), int64);
              if (account._isOnline && !AccountManager.getInstance()._accounts.ContainsKey(int64))
              {
                account.setOnlineStatus(false);
                account._status.ResetData(account.player_id);
              }
              accountList.Add(account);
            }
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
      return accountList;
    }

    public static List<PointBlank.Auth.Data.Model.Account> getClanPlayers(
      int clanId,
      long exception,
      bool isOnline)
    {
      List<PointBlank.Auth.Data.Model.Account> accountList = new List<PointBlank.Auth.Data.Model.Account>();
      if (clanId <= 0)
        return accountList;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@clan", (object) clanId);
          command.Parameters.AddWithValue("@on", (object) isOnline);
          command.CommandText = "SELECT player_id,player_name,rank,online,status FROM accounts WHERE clan_id=@clan AND online=@on";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            long int64 = npgsqlDataReader.GetInt64(0);
            if (int64 != exception)
            {
              PointBlank.Auth.Data.Model.Account account = new PointBlank.Auth.Data.Model.Account() { player_id = int64, player_name = npgsqlDataReader.GetString(1), _rank = npgsqlDataReader.GetInt32(2), _isOnline = npgsqlDataReader.GetBoolean(3) };
              account._status.SetData((uint) npgsqlDataReader.GetInt64(4), int64);
              if (account._isOnline && !AccountManager.getInstance()._accounts.ContainsKey(int64))
              {
                account.setOnlineStatus(false);
                account._status.ResetData(account.player_id);
              }
              accountList.Add(account);
            }
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
      return accountList;
    }
  }
}
