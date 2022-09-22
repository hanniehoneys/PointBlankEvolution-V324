using Npgsql;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Xml
{
  public class MissionsXml
  {
    private static List<MissionModel> Missions = new List<MissionModel>();
    public static uint _missionPage1;
    public static uint _missionPage2;

    public static void Load()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM info_missions ORDER BY mission_id ASC";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            bool boolean = npgsqlDataReader.GetBoolean(2);
            MissionModel missionModel = new MissionModel() { id = npgsqlDataReader.GetInt32(0), price = npgsqlDataReader.GetInt32(1) };
            uint num1 = (uint) (1 << missionModel.id);
            int num2 = (int) Math.Ceiling((double) missionModel.id / 32.0);
            if (boolean)
            {
              if (num2 == 1)
                MissionsXml._missionPage1 += num1;
              else if (num2 == 2)
                MissionsXml._missionPage2 += num1;
            }
            MissionsXml.Missions.Add(missionModel);
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
    }

    public static int GetMissionPrice(int id)
    {
      for (int index = 0; index < MissionsXml.Missions.Count; ++index)
      {
        MissionModel mission = MissionsXml.Missions[index];
        if (mission.id == id)
          return mission.price;
      }
      return -1;
    }
  }
}
