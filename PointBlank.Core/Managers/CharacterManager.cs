using Npgsql;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Managers
{
  public class CharacterManager
  {
    public static List<Character> getCharacters(long PlayerId)
    {
      if (PlayerId == 0L)
        return (List<Character>) null;
      List<Character> characterList = new List<Character>();
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@PlayerId", (object) PlayerId);
          command.CommandText = "SELECT * FROM player_characters WHERE player_id=@PlayerId ORDER BY slot ASC;";
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
            characterList.Add(new Character()
            {
              ObjId = npgsqlDataReader.GetInt64(0),
              Id = npgsqlDataReader.GetInt32(2),
              Slot = npgsqlDataReader.GetInt32(3),
              Name = npgsqlDataReader.GetString(4),
              CreateDate = npgsqlDataReader.GetInt32(5),
              PlayTime = npgsqlDataReader.GetInt32(6)
            });
          command.Dispose();
          npgsqlDataReader.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error("was a problem Loading (Character)!\r\n" + ex.ToString());
      }
      return characterList;
    }

    public static bool Create(Character Model, long PlayerId)
    {
      if (PlayerId == 0L)
        return false;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@player_id", (object) PlayerId);
          command.Parameters.AddWithValue("@id", (object) Model.Id);
          command.Parameters.AddWithValue("@slot", (object) Model.Slot);
          command.Parameters.AddWithValue("@name", (object) Model.Name);
          command.Parameters.AddWithValue("@createdate", (object) Model.CreateDate);
          command.Parameters.AddWithValue("@playtime", (object) Model.PlayTime);
          command.CommandType = CommandType.Text;
          command.CommandText = "INSERT INTO player_characters(player_id, id, slot, name, createdate, playtime)VALUES(@player_id, @id, @slot, @name, @createdate, @playtime) RETURNING object_id";
          object obj = command.ExecuteScalar();
          Model.ObjId = (long) obj;
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
          return true;
        }
      }
      catch (Exception ex)
      {
        Logger.error("was a problem Create (Characater)!\r\n" + ex.ToString());
        return false;
      }
    }

    public static bool Delete(long ObjectId, long PlayerId)
    {
      if (ObjectId == 0L || PlayerId == 0L)
        return false;
      return ComDiv.deleteDB("player_characters", "object_id", (object) ObjectId, "player_id", (object) PlayerId);
    }

    public static void Update(int Slot, long ObjectId)
    {
      if (ObjectId == 0L)
        return;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@ObjectId", (object) ObjectId);
          command.Parameters.AddWithValue("@Slot", (object) Slot);
          command.CommandText = "UPDATE player_characters SET slot=@Slot WHERE object_id=@ObjectId";
          command.CommandType = CommandType.Text;
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error("was a problem Update (Characater)!\r\n" + ex.ToString());
      }
    }

    public static int getSlots(long PlayerId)
    {
      int num = 0;
      if (PlayerId == 0L)
        return num;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@PlayerId", (object) PlayerId);
          command.CommandText = "SELECT slot FROM player_characters WHERE player_id=@PlayerId ORDER BY slot ASC;";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
            num = npgsqlDataReader.GetInt32(0);
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error("was a problem Slots (Characater)!\r\n" + ex.ToString());
      }
      return num;
    }
  }
}
