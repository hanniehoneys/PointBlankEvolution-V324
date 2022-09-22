using Npgsql;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Sql;
using PointBlank.Game.Data.Configs;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Game.Data.Managers
{
  public class GameRuleManager
  {
    public static List<GameRule> GameRules = new List<GameRule>();

    public static List<GameRule> getGameRules(int RuleId)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.Parameters.AddWithValue("@RuleId", (object) RuleId);
          command.CommandText = "SELECT * FROM gamerules WHERE id=@RuleId";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
            GameRuleManager.GameRules.Add(new GameRule()
            {
              WeaponId = npgsqlDataReader.GetInt32(1)
            });
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch
      {
      }
      return GameRuleManager.GameRules;
    }

    public static void Reload()
    {
      GameRuleManager.GameRules.Clear();
      GameRuleManager.getGameRules(GameConfig.ruleId);
    }
  }
}
