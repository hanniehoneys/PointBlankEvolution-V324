using Npgsql;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Sql;
using PointBlank.Core.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace PointBlank.Core.Network
{
  public static class ComDiv
  {
    public static int GetItemCategory(int id)
    {
      int idStatics = ComDiv.getIdStatics(id, 1);
      int num = idStatics % 10000;
      if (idStatics >= 1 && idStatics <= 5)
        return 1;
      if (idStatics >= 6 && idStatics <= 8 || idStatics == 15 || idStatics >= 26 && idStatics <= 27 || num >= 7 && num <= 14)
        return 2;
      if (idStatics >= 16 && idStatics <= 20)
        return 3;
      Logger.warning("Invalid Category [" + (object) idStatics + "]: " + (object) id);
      return 0;
    }

    public static int getIdStatics(int id, int type)
    {
      switch (type)
      {
        case 1:
          return id / 100000;
        case 2:
          return id % 100000 / 1000;
        case 3:
          return id % 1000;
        case 4:
          return id % 10000000 / 100000;
        default:
          return 0;
      }
    }

    public static int CreateItemId(int ItemClass, int ClassType, int Number)
    {
      return ItemClass * 100000 + ClassType * 1000 + Number;
    }

    public static ClassType getIdClassType(int id)
    {
      return (ClassType) (id % 100000 / 1000);
    }

    public static string arrayToString(byte[] buffer, int length)
    {
      string str = "";
      try
      {
        str = Encoding.Unicode.GetString(buffer, 0, length);
        int length1 = str.IndexOf(char.MinValue);
        if (length1 != -1)
          str = str.Substring(0, length1);
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
      return str;
    }

    private static int ToByte(char c)
    {
      if (c >= '0' && c <= '9')
        return (int) c - 48;
      if (c >= 'A' && c <= 'F')
        return (int) c - 65 + 10;
      if (c >= 'a' && c <= 'f')
        return (int) c - 97 + 10;
      return 0;
    }

    public static byte[] HexStringToByteArray(string hexString)
    {
      string source = hexString.Replace(":", "").Replace("-", "").Replace(" ", "");
      int length = source.Length;
      byte[] numArray = new byte[length / 2];
      for (int index = 0; index < length; index += 2)
        numArray[index / 2] = (byte) (ComDiv.ToByte(source.ElementAt<char>(index)) << 4 | ComDiv.ToByte(source.ElementAt<char>(index + 1)));
      return numArray;
    }

    public static bool updateDB(string TABELA, string COLUNA, object VALOR)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@valor", VALOR);
          command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error("[AllUtils.updateDB1] " + ex.ToString());
        return false;
      }
    }

    public static bool updateDB(
      string TABELA,
      string req1,
      object valorReq1,
      string[] COLUNAS,
      params object[] VALORES)
    {
      if (COLUNAS.Length != 0 && VALORES.Length != 0 && COLUNAS.Length != VALORES.Length)
      {
        Logger.error("[Update Database] Wrong values: " + string.Join(",", COLUNAS) + "/" + string.Join(",", VALORES));
        return false;
      }
      if (COLUNAS.Length == 0 || VALORES.Length == 0)
        return false;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          List<string> stringList = new List<string>();
          for (int index = 0; index < VALORES.Length; ++index)
          {
            object obj = VALORES[index];
            string str = COLUNAS[index];
            string parameterName = "@valor" + (object) index;
            command.Parameters.AddWithValue(parameterName, obj);
            stringList.Add(str + "=" + parameterName);
          }
          string str1 = string.Join(",", stringList.ToArray());
          command.Parameters.AddWithValue("@req1", valorReq1);
          command.CommandText = "UPDATE " + TABELA + " SET " + str1 + " WHERE " + req1 + "=@req1";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error("[AllUtils.updateDB2] " + ex.ToString());
        return false;
      }
    }

    public static bool updateDB(
      string TABELA,
      string COLUNA,
      object VALOR,
      string req1,
      object valorReq1)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@valor", VALOR);
          command.Parameters.AddWithValue("@req1", valorReq1);
          command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor WHERE " + req1 + "=@req1";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error("[AllUtils.updateDB3] " + ex.ToString());
        return false;
      }
    }

    public static bool updateDB(
      string TABELA,
      string req1,
      object valorReq1,
      string req2,
      object valorReq2,
      string[] COLUNAS,
      params object[] VALORES)
    {
      if (COLUNAS.Length != 0 && VALORES.Length != 0 && COLUNAS.Length != VALORES.Length)
      {
        Logger.error("[Update Database] Wrong values: " + string.Join(",", COLUNAS) + "/" + string.Join(",", VALORES));
        return false;
      }
      if (COLUNAS.Length == 0 || VALORES.Length == 0)
        return false;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          List<string> stringList = new List<string>();
          for (int index = 0; index < VALORES.Length; ++index)
          {
            object obj = VALORES[index];
            string str = COLUNAS[index];
            string parameterName = "@valor" + (object) index;
            command.Parameters.AddWithValue(parameterName, obj);
            stringList.Add(str + "=" + parameterName);
          }
          string str1 = string.Join(",", stringList.ToArray());
          if (req1 != null)
            command.Parameters.AddWithValue("@req1", valorReq1);
          if (req2 != null)
            command.Parameters.AddWithValue("@req2", valorReq2);
          if (req1 != null && req2 == null)
            command.CommandText = "UPDATE " + TABELA + " SET " + str1 + " WHERE " + req1 + "=@req1";
          else if (req2 != null && req1 == null)
            command.CommandText = "UPDATE " + TABELA + " SET " + str1 + " WHERE " + req2 + "=@req2";
          else if (req2 != null && req1 != null)
            command.CommandText = "UPDATE " + TABELA + " SET " + str1 + " WHERE " + req1 + "=@req1 AND " + req2 + "=@req2";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error("[AllUtils.updateDB4] " + ex.ToString());
        return false;
      }
    }

    public static bool updateDB(
      string TABELA,
      string req1,
      int[] valorReq1,
      string req2,
      object valorReq2,
      string[] COLUNAS,
      params object[] VALORES)
    {
      if (COLUNAS.Length != 0 && VALORES.Length != 0 && COLUNAS.Length != VALORES.Length)
      {
        Logger.error("[updateDB5] Wrong values: " + string.Join(",", COLUNAS) + "/" + string.Join(",", VALORES));
        return false;
      }
      if (COLUNAS.Length == 0 || VALORES.Length == 0)
        return false;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          List<string> stringList = new List<string>();
          for (int index = 0; index < VALORES.Length; ++index)
          {
            object obj = VALORES[index];
            string str = COLUNAS[index];
            string parameterName = "@valor" + (object) index;
            command.Parameters.AddWithValue(parameterName, obj);
            stringList.Add(str + "=" + parameterName);
          }
          string str1 = string.Join(",", stringList.ToArray());
          if (req1 != null)
            command.Parameters.AddWithValue("@req1", (object) valorReq1);
          if (req2 != null)
            command.Parameters.AddWithValue("@req2", valorReq2);
          if (req1 != null && req2 == null)
            command.CommandText = "UPDATE " + TABELA + " SET " + str1 + " WHERE " + req1 + " = ANY (@req1)";
          else if (req2 != null && req1 == null)
            command.CommandText = "UPDATE " + TABELA + " SET " + str1 + " WHERE " + req2 + "=@req2";
          else if (req2 != null && req1 != null)
            command.CommandText = "UPDATE " + TABELA + " SET " + str1 + " WHERE " + req1 + " = ANY (@req1) AND " + req2 + "=@req2";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error("[AllUtils.updateDB5] " + ex.ToString());
        return false;
      }
    }

    public static bool updateDB(
      string TABELA,
      string COLUNA,
      object VALOR,
      string req1,
      object valorReq1,
      string req2,
      object valorReq2)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@valor", VALOR);
          if (req1 != null)
            command.Parameters.AddWithValue("@req1", valorReq1);
          if (req2 != null)
            command.Parameters.AddWithValue("@req2", valorReq2);
          if (req1 != null && req2 == null)
            command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor WHERE " + req1 + "=@req1";
          else if (req2 != null && req1 == null)
            command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor WHERE " + req2 + "=@req2";
          else if (req2 != null && req1 != null)
            command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor WHERE " + req1 + "=@req1 AND " + req2 + "=@req2";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error("[AllUtils.updateDB6] " + ex.ToString());
        return false;
      }
    }

    public static bool deleteDB(string TABELA, string req1, object valorReq1)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@req1", valorReq1);
          command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req1 + "=@req1";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
        return false;
      }
    }

    public static bool deleteDB(
      string TABELA,
      string req1,
      object valorReq1,
      string req2,
      object valorReq2)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          if (req1 != null)
            command.Parameters.AddWithValue("@req1", valorReq1);
          if (req2 != null)
            command.Parameters.AddWithValue("@req2", valorReq2);
          if (req1 != null && req2 == null)
            command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req1 + "=@req1";
          else if (req2 != null && req1 == null)
            command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req2 + "=@req2";
          else if (req2 != null && req1 != null)
            command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req1 + "=@req1 AND " + req2 + "=@req2";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
        return false;
      }
    }

    public static bool deleteDB(
      string TABELA,
      string req1,
      object[] valorReq1,
      string req2,
      object valorReq2)
    {
      if (valorReq1.Length == 0)
        return false;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandType = CommandType.Text;
          List<string> stringList = new List<string>();
          for (int index = 0; index < valorReq1.Length; ++index)
          {
            object obj = valorReq1[index];
            string parameterName = "@valor" + (object) index;
            command.Parameters.AddWithValue(parameterName, obj);
            stringList.Add(parameterName);
          }
          string str = string.Join(",", stringList.ToArray());
          command.Parameters.AddWithValue("@req2", valorReq2);
          command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req1 + " in (" + str + ") AND " + req2 + "=@req2";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
        return false;
      }
    }

    public static byte[] Decrypt(byte[] data, int shift)
    {
      byte[] numArray = new byte[data.Length];
      Array.Copy((Array) data, 0, (Array) numArray, 0, numArray.Length);
      byte num = numArray[numArray.Length - 1];
      for (int index = numArray.Length - 1; index > 0; --index)
        numArray[index] = (byte) (((int) numArray[index - 1] & (int) byte.MaxValue) << 8 - shift | ((int) numArray[index] & (int) byte.MaxValue) >> shift);
      numArray[0] = (byte) ((int) num << 8 - shift | ((int) numArray[0] & (int) byte.MaxValue) >> shift);
      return numArray;
    }

    public static DateTime GetLinkerTime(Assembly assembly, TimeZoneInfo target = null)
    {
      string location = assembly.Location;
      byte[] buffer = new byte[2048];
      using (FileStream fileStream = new FileStream(location, FileMode.Open, FileAccess.Read))
        fileStream.Read(buffer, 0, 2048);
      int int32 = BitConverter.ToInt32(buffer, 60);
      return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double) BitConverter.ToInt32(buffer, int32 + 8)), target ?? TimeZoneInfo.Local);
    }

    public static ushort getCardFlags(int missionId, int cardIdx, byte[] arrayList)
    {
      if (missionId == 0)
        return 0;
      int num = 0;
      List<Card> cards = MissionCardXml.getCards(missionId, cardIdx);
      for (int index = 0; index < cards.Count; ++index)
      {
        Card card = cards[index];
        if ((int) arrayList[card._arrayIdx] >= card._missionLimit)
          num |= card._flag;
      }
      return (ushort) num;
    }

    public static byte[] getCardFlags(int missionId, byte[] arrayList)
    {
      if (missionId == 0)
        return new byte[20];
      List<Card> cards1 = MissionCardXml.getCards(missionId);
      if (cards1.Count == 0)
        return new byte[20];
      using (SendGPacket sendGpacket = new SendGPacket(20L))
      {
        int num = 0;
        for (int cardBasicId = 0; cardBasicId < 10; ++cardBasicId)
        {
          List<Card> cards2 = MissionCardXml.getCards(cards1, cardBasicId);
          for (int index = 0; index < cards2.Count; ++index)
          {
            Card card = cards2[index];
            if ((int) arrayList[card._arrayIdx] >= card._missionLimit)
              num |= card._flag;
          }
          sendGpacket.writeH((ushort) num);
          num = 0;
        }
        return sendGpacket.mstream.ToArray();
      }
    }

    public static uint GetPlayerStatus(AccountStatus status, bool isOnline)
    {
      FriendState state;
      int roomId;
      int channelId;
      int serverId;
      ComDiv.GetPlayerLocation(status, isOnline, out state, out roomId, out channelId, out serverId);
      return ComDiv.GetPlayerStatus(roomId, channelId, serverId, (int) state);
    }

    public static uint GetPlayerStatus(int roomId, int channelId, int serverId, int stateId)
    {
      return (uint) ((stateId & (int) byte.MaxValue) << 28 | (serverId & (int) byte.MaxValue) << 20 | (channelId & (int) byte.MaxValue) << 12 | roomId & 4095);
    }

    public static ulong GetPlayerStatus(
      int clanFId,
      int roomId,
      int channelId,
      int serverId,
      int stateId)
    {
      return (ulong) (((long) clanFId & (long) uint.MaxValue) << 32) | (ulong) ((stateId & (int) byte.MaxValue) << 28) | (ulong) ((serverId & (int) byte.MaxValue) << 20) | (ulong) ((channelId & (int) byte.MaxValue) << 12) | (ulong) (roomId & 4095);
    }

    public static ulong GetClanStatus(AccountStatus status, bool isOnline)
    {
      FriendState state;
      int roomId;
      int channelId;
      int serverId;
      int clanFId;
      ComDiv.GetPlayerLocation(status, isOnline, out state, out roomId, out channelId, out serverId, out clanFId);
      return ComDiv.GetPlayerStatus(clanFId, roomId, channelId, serverId, (int) state);
    }

    public static ulong GetClanStatus(FriendState state)
    {
      return ComDiv.GetPlayerStatus(0, 0, 0, 0, (int) state);
    }

    public static uint GetFriendStatus(Friend f)
    {
      PlayerInfo player = f.player;
      if (player == null)
        return 0;
      FriendState state = FriendState.None;
      int serverId = 0;
      int channelId = 0;
      int roomId = 0;
      if (f.removed)
        state = FriendState.Offline;
      else if (f.state > 0)
        state = (FriendState) f.state;
      else
        ComDiv.GetPlayerLocation(player._status, player._isOnline, out state, out roomId, out channelId, out serverId);
      return ComDiv.GetPlayerStatus(roomId, channelId, serverId, (int) state);
    }

    public static uint GetFriendStatus(Friend f, FriendState stateN)
    {
      PlayerInfo player = f.player;
      if (player == null)
        return 0;
      FriendState state = stateN;
      int serverId = 0;
      int channelId = 0;
      int roomId = 0;
      if (f.removed)
        state = FriendState.Offline;
      else if (f.state > 0)
        state = (FriendState) f.state;
      else if (stateN == FriendState.None)
        ComDiv.GetPlayerLocation(player._status, player._isOnline, out state, out roomId, out channelId, out serverId);
      return ComDiv.GetPlayerStatus(roomId, channelId, serverId, (int) state);
    }

    public static void GetPlayerLocation(
      AccountStatus status,
      bool isOnline,
      out FriendState state,
      out int roomId,
      out int channelId,
      out int serverId)
    {
      roomId = 0;
      channelId = 0;
      serverId = 0;
      if (isOnline)
      {
        if (status.roomId != byte.MaxValue)
        {
          roomId = (int) status.roomId;
          channelId = (int) status.channelId;
          state = FriendState.Room;
        }
        else if (status.roomId == byte.MaxValue && status.channelId != byte.MaxValue)
        {
          channelId = (int) status.channelId;
          state = FriendState.Lobby;
        }
        else
        {
          int num = status.roomId != byte.MaxValue ? 0 : (status.channelId == byte.MaxValue ? 1 : 0);
          state = num == 0 ? FriendState.Offline : FriendState.Online;
        }
        if (status.serverId == byte.MaxValue)
          return;
        serverId = (int) status.serverId;
      }
      else
        state = FriendState.Offline;
    }

    public static void GetPlayerLocation(
      AccountStatus status,
      bool isOnline,
      out FriendState state,
      out int roomId,
      out int channelId,
      out int serverId,
      out int clanFId)
    {
      roomId = 0;
      channelId = 0;
      serverId = 0;
      clanFId = 0;
      if (isOnline)
      {
        if (status.roomId != byte.MaxValue)
        {
          roomId = (int) status.roomId;
          channelId = (int) status.channelId;
          state = FriendState.Room;
        }
        else if ((status.clanFId != byte.MaxValue || status.roomId == byte.MaxValue) && status.channelId != byte.MaxValue)
        {
          channelId = (int) status.channelId;
          state = FriendState.Lobby;
        }
        else
        {
          int num = status.roomId != byte.MaxValue ? 0 : (status.channelId == byte.MaxValue ? 1 : 0);
          state = num == 0 ? FriendState.Offline : FriendState.Online;
        }
        if (status.serverId != byte.MaxValue)
          serverId = (int) status.serverId;
        if (status.clanFId == byte.MaxValue)
          return;
        clanFId = (int) status.clanFId + 1;
      }
      else
        state = FriendState.Offline;
    }

    public static string gen5(string text)
    {
      using (HMACMD5 hmacmD5 = new HMACMD5(Encoding.UTF8.GetBytes("/x!a@r-$r%an¨.&e&+f*f(f(a)")))
      {
        byte[] hash = hmacmD5.ComputeHash(Encoding.UTF8.GetBytes(text));
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < hash.Length; ++index)
          stringBuilder.Append(hash[index].ToString("x2"));
        return stringBuilder.ToString();
      }
    }
  }
}
