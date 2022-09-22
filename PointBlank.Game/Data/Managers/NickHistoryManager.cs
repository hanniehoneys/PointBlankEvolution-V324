using Npgsql;
using PointBlank.Core;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Game.Data.Managers
{
  public static class NickHistoryManager
  {
    public static List<NHistoryModel> getHistory(object valor, int type)
    {
      List<NHistoryModel> nhistoryModelList = new List<NHistoryModel>();
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          string str = type == 0 ? "WHERE to_nick=@valor" : "WHERE player_id=@valor";
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@valor", valor);
          command.CommandText = "SELECT * FROM nick_history " + str + " ORDER BY change_date LIMIT 30";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
            nhistoryModelList.Add(new NHistoryModel()
            {
              player_id = npgsqlDataReader.GetInt64(0),
              from_nick = npgsqlDataReader.GetString(1),
              to_nick = npgsqlDataReader.GetString(2),
              date = (uint) npgsqlDataReader.GetInt64(3),
              motive = npgsqlDataReader.GetString(4)
            });
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch
      {
        Logger.error("Ocorreu um problema ao carregar o histórico de apelidos!");
      }
      return nhistoryModelList;
    }

    public static bool CreateHistory(
      long player_id,
      string old_nick,
      string new_nick,
      string motive)
    {
      NHistoryModel nhistoryModel = new NHistoryModel()
      {
        player_id = player_id,
        from_nick = old_nick,
        to_nick = new_nick,
        date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm")),
        motive = motive
      };
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@owner", (object) nhistoryModel.player_id);
          command.Parameters.AddWithValue("@oldnick", (object) nhistoryModel.from_nick);
          command.Parameters.AddWithValue("@newnick", (object) nhistoryModel.to_nick);
          command.Parameters.AddWithValue("@date", (object) (long) nhistoryModel.date);
          command.Parameters.AddWithValue("@motive", (object) nhistoryModel.motive);
          command.CommandType = CommandType.Text;
          command.CommandText = "INSERT INTO nick_history(player_id,from_nick,to_nick,change_date,motive)VALUES(@owner,@oldnick,@newnick,@date,@motive)";
          command.ExecuteNonQuery();
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
  }
}
