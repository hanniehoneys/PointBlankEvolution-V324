using Npgsql;
using PointBlank.Core.Models.Account.Title;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Data;

namespace PointBlank.Core.Managers
{
  public class TitleManager
  {
    private static TitleManager acm = new TitleManager();

    public static TitleManager getInstance()
    {
      return TitleManager.acm;
    }

    public bool CreateTitleDB(long player_id)
    {
      if (player_id == 0L)
        return false;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@owner", (object) player_id);
          command.CommandText = "INSERT INTO player_titles (owner_id) VALUES (@owner)";
          command.CommandType = CommandType.Text;
          command.ExecuteNonQuery();
          command.Dispose();
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

    public PlayerTitles getTitleDB(long pId)
    {
      PlayerTitles playerTitles = new PlayerTitles();
      if (pId == 0L)
        return playerTitles;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@owner", (object) pId);
          command.CommandText = "SELECT * FROM player_titles WHERE owner_id=@owner";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            playerTitles.ownerId = pId;
            playerTitles.Equiped1 = npgsqlDataReader.GetInt32(1);
            playerTitles.Equiped2 = npgsqlDataReader.GetInt32(2);
            playerTitles.Equiped3 = npgsqlDataReader.GetInt32(3);
            playerTitles.Flags = npgsqlDataReader.GetInt64(4);
            playerTitles.Slots = npgsqlDataReader.GetInt32(5);
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error("Ocorreu um problema ao carregar os títulos!\r\n" + ex.ToString());
      }
      return playerTitles;
    }

    public bool updateEquipedTitle(long player_id, int index, int titleId)
    {
      return ComDiv.updateDB("player_titles", "titleequiped" + (object) (index + 1), (object) titleId, "owner_id", (object) player_id);
    }

    public void updateTitlesFlags(long player_id, long flags)
    {
      ComDiv.updateDB("player_titles", "titleflags", (object) flags, "owner_id", (object) player_id);
    }

    public void updateRequi(
      long player_id,
      int medalhas,
      int insignias,
      int ordens_azuis,
      int broche)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@pid", (object) player_id);
          command.Parameters.AddWithValue("@broche", (object) broche);
          command.Parameters.AddWithValue("@insignias", (object) insignias);
          command.Parameters.AddWithValue("@medalhas", (object) medalhas);
          command.Parameters.AddWithValue("@ordensazuis", (object) ordens_azuis);
          command.CommandType = CommandType.Text;
          command.CommandText = "UPDATE accounts SET brooch=@broche, insignia=@insignias, medal=@medalhas, blue_order=@ordensazuis WHERE player_id=@pid";
          command.ExecuteNonQuery();
          command.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }
  }
}
