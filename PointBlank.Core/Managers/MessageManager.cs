using Npgsql;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Managers
{
  public static class MessageManager
  {
    public static Message getMessage(int objId, long pId)
    {
      Message message = (Message) null;
      if (pId == 0L || objId == 0)
        return (Message) null;
      try
      {
        DateTime now = DateTime.Now;
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@obj", (object) objId);
          command.Parameters.AddWithValue("@owner", (object) pId);
          command.CommandText = "SELECT * FROM player_messages WHERE object_id=@obj AND owner_id=@owner";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
            message = new Message(npgsqlDataReader.GetInt64(8), now)
            {
              object_id = objId,
              sender_id = npgsqlDataReader.GetInt64(2),
              clanId = npgsqlDataReader.GetInt32(3),
              sender_name = npgsqlDataReader.GetString(4),
              text = npgsqlDataReader.GetString(5),
              type = npgsqlDataReader.GetInt32(6),
              state = npgsqlDataReader.GetInt32(7),
              cB = (NoteMessageClan) npgsqlDataReader.GetInt32(9)
            };
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
        return (Message) null;
      }
      return message;
    }

    public static List<Message> getGifts(long owner_id)
    {
      List<Message> messageList = new List<Message>();
      if (owner_id == 0L)
        return messageList;
      try
      {
        DateTime now = DateTime.Now;
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@owner", (object) owner_id);
          command.CommandText = "SELECT * FROM player_messages WHERE owner_id=@owner";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            int int32 = npgsqlDataReader.GetInt32(6);
            if (int32 == 2)
            {
              Message message = new Message(npgsqlDataReader.GetInt64(8), now) { object_id = npgsqlDataReader.GetInt32(0), sender_id = npgsqlDataReader.GetInt64(2), clanId = npgsqlDataReader.GetInt32(3), sender_name = npgsqlDataReader.GetString(4), text = npgsqlDataReader.GetString(5), type = int32, state = npgsqlDataReader.GetInt32(7), cB = (NoteMessageClan) npgsqlDataReader.GetInt32(9) };
              messageList.Add(message);
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
      return messageList;
    }

    public static List<Message> getMessages(long owner_id)
    {
      List<Message> messageList = new List<Message>();
      if (owner_id == 0L)
        return messageList;
      try
      {
        DateTime now = DateTime.Now;
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@owner", (object) owner_id);
          command.CommandText = "SELECT * FROM player_messages WHERE owner_id=@owner";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            int int32 = npgsqlDataReader.GetInt32(6);
            if (int32 != 2)
            {
              Message message = new Message(npgsqlDataReader.GetInt64(8), now) { object_id = npgsqlDataReader.GetInt32(0), sender_id = npgsqlDataReader.GetInt64(2), clanId = npgsqlDataReader.GetInt32(3), sender_name = npgsqlDataReader.GetString(4), text = npgsqlDataReader.GetString(5), type = int32, state = npgsqlDataReader.GetInt32(7), cB = (NoteMessageClan) npgsqlDataReader.GetInt32(9) };
              messageList.Add(message);
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
      return messageList;
    }

    public static bool messageExists(int objId, long owner_id)
    {
      if (owner_id == 0L || objId == 0)
        return false;
      try
      {
        int num = 0;
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@obj", (object) objId);
          command.Parameters.AddWithValue("@owner", (object) owner_id);
          command.CommandText = "SELECT COUNT(*) FROM player_messages WHERE object_id=@obj AND owner_id=@owner";
          num = Convert.ToInt32(command.ExecuteScalar());
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
        return num > 0;
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return false;
    }

    public static int getMsgsCount(long owner_id)
    {
      int num = 0;
      if (owner_id == 0L)
        return num;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@owner", (object) owner_id);
          command.CommandText = "SELECT COUNT(*) FROM player_messages WHERE owner_id=@owner";
          num = Convert.ToInt32(command.ExecuteScalar());
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return num;
    }

    public static bool CreateMessage(long owner_id, Message msg)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@owner", (object) owner_id);
          command.Parameters.AddWithValue("@sendid", (object) msg.sender_id);
          command.Parameters.AddWithValue("@clan", (object) msg.clanId);
          command.Parameters.AddWithValue("@sendname", (object) msg.sender_name);
          command.Parameters.AddWithValue("@text", (object) msg.text);
          command.Parameters.AddWithValue("@type", (object) msg.type);
          command.Parameters.AddWithValue("@state", (object) msg.state);
          command.Parameters.AddWithValue("@expire", (object) msg.expireDate);
          command.Parameters.AddWithValue("@cb", (object) (int) msg.cB);
          command.CommandType = CommandType.Text;
          command.CommandText = "INSERT INTO player_messages(owner_id,sender_id,clan_id,sender_name,text,type,state,expire,cb)VALUES(@owner,@sendid,@clan,@sendname,@text,@type,@state,@expire,@cb) RETURNING object_id";
          object obj = command.ExecuteScalar();
          msg.object_id = (int) obj;
          command.Dispose();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
          return true;
        }
      }
      catch
      {
        return false;
      }
    }

    public static void updateState(int objId, long owner, int value)
    {
      ComDiv.updateDB("player_messages", "state", (object) value, "object_id", (object) objId, "owner_id", (object) owner);
    }

    public static void updateExpireDate(int objId, long owner, long date)
    {
      ComDiv.updateDB("player_messages", "expire", (object) date, "object_id", (object) objId, "owner_id", (object) owner);
    }

    public static bool DeleteMessage(int objId, long owner)
    {
      if (owner == 0L || objId == 0)
        return false;
      return ComDiv.deleteDB("player_messages", "object_id", (object) objId, "owner_id", (object) owner);
    }

    public static bool DeleteMessages(List<object> objs, long owner)
    {
      if (owner == 0L || objs.Count == 0)
        return false;
      return ComDiv.deleteDB("player_messages", "object_id", objs.ToArray(), "owner_id", (object) owner);
    }

    public static void RecicleMessages(long owner_id, List<Message> msgs)
    {
      List<object> objs = new List<object>();
      for (int index = 0; index < msgs.Count; ++index)
      {
        Message msg = msgs[index];
        if (msg.DaysRemaining == 0)
        {
          objs.Add((object) msg.object_id);
          msgs.RemoveAt(index--);
        }
      }
      MessageManager.DeleteMessages(objs, owner_id);
    }
  }
}
