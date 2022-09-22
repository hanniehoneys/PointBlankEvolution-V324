using Npgsql;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Data;

namespace PointBlank.Core.Managers.Server
{
  public static class ServerConfigSyncer
  {
    public static ServerConfig GenerateConfig(int configId)
    {
      ServerConfig serverConfig = (ServerConfig) null;
      if (configId == 0)
        return serverConfig;
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@cfg", (object) configId);
          command.CommandText = "SELECT * FROM info_login_configs WHERE config_id=@cfg";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
            serverConfig = new ServerConfig()
            {
              configId = configId,
              onlyGM = npgsqlDataReader.GetBoolean(1),
              missions = npgsqlDataReader.GetBoolean(2),
              UserFileList = npgsqlDataReader.GetString(3),
              ClientVersion = npgsqlDataReader.GetString(4),
              GiftSystem = npgsqlDataReader.GetBoolean(5),
              ExitURL = npgsqlDataReader.GetString(6),
              ChatColor = npgsqlDataReader.GetInt32(7),
              AnnouceColor = npgsqlDataReader.GetInt32(8),
              Chat = npgsqlDataReader.GetString(9),
              Annouce = npgsqlDataReader.GetString(10)
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
      }
      return serverConfig;
    }

    public static bool updateMission(ServerConfig cfg, bool mission)
    {
      cfg.missions = mission;
      return ComDiv.updateDB("info_login_configs", "missions", (object) mission, "config_id", (object) cfg.configId);
    }
  }
}
