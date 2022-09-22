using Npgsql;
using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Game.Data.Managers
{
  public static class ClanManager
  {
    public static List<PointBlank.Core.Models.Account.Clan.Clan> _clans = new List<PointBlank.Core.Models.Account.Clan.Clan>();

    public static void Load()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM clan_data";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            long int64 = npgsqlDataReader.GetInt64(3);
            if (int64 != 0L)
            {
              PointBlank.Core.Models.Account.Clan.Clan clan = new PointBlank.Core.Models.Account.Clan.Clan()
              {
                _id = npgsqlDataReader.GetInt32(0),
                _rank = npgsqlDataReader.GetInt32(1),
                _name = npgsqlDataReader.GetString(2),
                owner_id = int64,
                _logo = (uint) npgsqlDataReader.GetInt64(4),
                _name_color = npgsqlDataReader.GetInt32(5),
                _info = npgsqlDataReader.GetString(6),
                _news = npgsqlDataReader.GetString(7),
                creationDate = npgsqlDataReader.GetInt32(8),
                autoridade = npgsqlDataReader.GetInt32(9),
                limite_rank = npgsqlDataReader.GetInt32(10),
                limite_idade = npgsqlDataReader.GetInt32(11),
                limite_idade2 = npgsqlDataReader.GetInt32(12),
                partidas = npgsqlDataReader.GetInt32(13),
                vitorias = npgsqlDataReader.GetInt32(14),
                derrotas = npgsqlDataReader.GetInt32(15),
                _pontos = npgsqlDataReader.GetFloat(16),
                maxPlayers = npgsqlDataReader.GetInt32(17),
                _exp = npgsqlDataReader.GetInt32(18),
                effect = npgsqlDataReader.GetInt32(24)
              };
              string Exp = npgsqlDataReader.GetString(19);
              string Part = npgsqlDataReader.GetString(20);
              string Wins = npgsqlDataReader.GetString(21);
              string Kills = npgsqlDataReader.GetString(22);
              string Hs = npgsqlDataReader.GetString(23);
              clan.BestPlayers.SetPlayers(Exp, Part, Wins, Kills, Hs);
              ClanManager._clans.Add(clan);
            }
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    public static List<PointBlank.Core.Models.Account.Clan.Clan> getClanListPerPage(int page)
    {
      List<PointBlank.Core.Models.Account.Clan.Clan> clanList = new List<PointBlank.Core.Models.Account.Clan.Clan>();
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@page", (object) (170 * page));
          command.CommandText = "SELECT * FROM clan_data ORDER BY clan_id DESC OFFSET @page LIMIT 170";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            long int64 = npgsqlDataReader.GetInt64(3);
            if (int64 != 0L)
            {
              PointBlank.Core.Models.Account.Clan.Clan clan = new PointBlank.Core.Models.Account.Clan.Clan()
              {
                _id = npgsqlDataReader.GetInt32(0),
                _rank = npgsqlDataReader.GetInt32(1),
                _name = npgsqlDataReader.GetString(2),
                owner_id = int64,
                _logo = (uint) npgsqlDataReader.GetInt64(4),
                _name_color = npgsqlDataReader.GetInt32(5),
                _info = npgsqlDataReader.GetString(6),
                _news = npgsqlDataReader.GetString(7),
                creationDate = npgsqlDataReader.GetInt32(8),
                autoridade = npgsqlDataReader.GetInt32(9),
                limite_rank = npgsqlDataReader.GetInt32(10),
                limite_idade = npgsqlDataReader.GetInt32(11),
                limite_idade2 = npgsqlDataReader.GetInt32(12),
                partidas = npgsqlDataReader.GetInt32(13),
                vitorias = npgsqlDataReader.GetInt32(14),
                derrotas = npgsqlDataReader.GetInt32(15),
                _pontos = npgsqlDataReader.GetFloat(16),
                maxPlayers = npgsqlDataReader.GetInt32(17),
                _exp = npgsqlDataReader.GetInt32(18),
                effect = npgsqlDataReader.GetInt32(24)
              };
              string Exp = npgsqlDataReader.GetString(19);
              string Part = npgsqlDataReader.GetString(20);
              string Wins = npgsqlDataReader.GetString(21);
              string Kills = npgsqlDataReader.GetString(22);
              string Hs = npgsqlDataReader.GetString(23);
              clan.BestPlayers.SetPlayers(Exp, Part, Wins, Kills, Hs);
              clanList.Add(clan);
            }
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return clanList;
    }

    public static PointBlank.Core.Models.Account.Clan.Clan getClan(int id)
    {
      if (id == 0)
        return new PointBlank.Core.Models.Account.Clan.Clan();
      lock (ClanManager._clans)
      {
        for (int index = 0; index < ClanManager._clans.Count; ++index)
        {
          PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager._clans[index];
          if (clan._id == id)
            return clan;
        }
      }
      return new PointBlank.Core.Models.Account.Clan.Clan();
    }

    public static List<PointBlank.Game.Data.Model.Account> getClanPlayers(
      int clan_id,
      long exception,
      bool useCache)
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      if (clan_id == 0)
        return accountList;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@clan", (object) clan_id);
          command.CommandText = "SELECT player_id, player_name, name_color, rank, online, clanaccess, clandate, status FROM accounts WHERE clan_id=@clan";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            long int64 = npgsqlDataReader.GetInt64(0);
            if (int64 != exception)
            {
              PointBlank.Game.Data.Model.Account account1 = new PointBlank.Game.Data.Model.Account()
              {
                player_id = int64,
                player_name = npgsqlDataReader.GetString(1),
                name_color = npgsqlDataReader.GetInt32(2),
                clanId = clan_id,
                _rank = npgsqlDataReader.GetInt32(3),
                _isOnline = npgsqlDataReader.GetBoolean(4),
                clanAccess = npgsqlDataReader.GetInt32(5),
                clanDate = npgsqlDataReader.GetInt32(6)
              };
              account1._status.SetData((uint) npgsqlDataReader.GetInt64(7), account1.player_id);
              if (useCache)
              {
                PointBlank.Game.Data.Model.Account account2 = AccountManager.getAccount(account1.player_id, true);
                if (account2 != null)
                  account1._connection = account2._connection;
              }
              accountList.Add(account1);
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
        Logger.error(ex.ToString());
      }
      return accountList;
    }

    public static List<PointBlank.Game.Data.Model.Account> getClanPlayers(
      int clan_id,
      long exception,
      bool useCache,
      bool isOnline)
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      if (clan_id == 0)
        return accountList;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@clan", (object) clan_id);
          command.Parameters.AddWithValue("@on", (object) isOnline);
          command.CommandText = "SELECT player_id, player_name, name_color, rank, clanaccess, clandate, status FROM accounts WHERE clan_id=@clan AND online=@on";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            long int64 = npgsqlDataReader.GetInt64(0);
            if (int64 != exception)
            {
              PointBlank.Game.Data.Model.Account account1 = new PointBlank.Game.Data.Model.Account()
              {
                player_id = int64,
                clanId = clan_id,
                player_name = npgsqlDataReader.GetString(1),
                name_color = npgsqlDataReader.GetInt32(2),
                _rank = npgsqlDataReader.GetInt32(3),
                _isOnline = isOnline,
                clanAccess = npgsqlDataReader.GetInt32(4),
                clanDate = npgsqlDataReader.GetInt32(5)
              };
              account1._status.SetData((uint) npgsqlDataReader.GetInt64(6), account1.player_id);
              if (useCache)
              {
                PointBlank.Game.Data.Model.Account account2 = AccountManager.getAccount(account1.player_id, true);
                if (account2 != null)
                  account1._connection = account2._connection;
              }
              accountList.Add(account1);
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
        Logger.error(ex.ToString());
      }
      return accountList;
    }

    public static void SendPacket(SendPacket packet, List<PointBlank.Game.Data.Model.Account> players)
    {
      if (players.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("ClanManager.SendPacket");
      for (int index = 0; index < players.Count; ++index)
        players[index].SendCompletePacket(completeBytes, false);
    }

    public static void SendPacket(SendPacket packet, List<PointBlank.Game.Data.Model.Account> players, long exception)
    {
      if (players.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("ClanManager.SendPacket");
      for (int index = 0; index < players.Count; ++index)
      {
        PointBlank.Game.Data.Model.Account player = players[index];
        if (player.player_id != exception)
          player.SendCompletePacket(completeBytes, false);
      }
    }

    public static void SendPacket(
      SendPacket packet,
      int clan_id,
      long exception,
      bool useCache,
      bool isOnline)
    {
      List<PointBlank.Game.Data.Model.Account> clanPlayers = ClanManager.getClanPlayers(clan_id, exception, useCache, isOnline);
      ClanManager.SendPacket(packet, clanPlayers);
    }

    public static bool RemoveClan(PointBlank.Core.Models.Account.Clan.Clan clan)
    {
      lock (ClanManager._clans)
        return ClanManager._clans.Remove(clan);
    }

    public static void AddClan(PointBlank.Core.Models.Account.Clan.Clan clan)
    {
      lock (ClanManager._clans)
        ClanManager._clans.Add(clan);
    }

    public static bool isClanNameExist(string name)
    {
      if (string.IsNullOrEmpty(name))
        return true;
      try
      {
        int num = 0;
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@name", (object) name);
          command.CommandText = "SELECT COUNT(*) FROM clan_data WHERE clan_name=@name";
          num = Convert.ToInt32(command.ExecuteScalar());
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
        return num > 0;
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
        return true;
      }
    }

    public static bool isClanLogoExist(uint logo)
    {
      try
      {
        int num = 0;
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@logo", (object) (long) logo);
          command.CommandText = "SELECT COUNT(*) FROM clan_data WHERE logo=@logo";
          num = Convert.ToInt32(command.ExecuteScalar());
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
        return num > 0;
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
        return true;
      }
    }
  }
}
