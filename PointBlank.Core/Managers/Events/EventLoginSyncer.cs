using Npgsql;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Managers.Events
{
  public class EventLoginSyncer
  {
    private static List<EventLoginModel> _events = new List<EventLoginModel>();

    public static void GenerateList()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM events_login";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            EventLoginModel eventLoginModel = new EventLoginModel() { startDate = (uint) npgsqlDataReader.GetInt64(0), endDate = (uint) npgsqlDataReader.GetInt64(1), _rewardId = npgsqlDataReader.GetInt32(2), _count = npgsqlDataReader.GetInt64(3) };
            eventLoginModel._category = ComDiv.GetItemCategory(eventLoginModel._rewardId);
            if (eventLoginModel._rewardId < 100000)
              Logger.error("Event with incorrect reward! [Id: " + (object) eventLoginModel._rewardId + "]");
            else
              EventLoginSyncer._events.Add(eventLoginModel);
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

    public static void ReGenList()
    {
      EventLoginSyncer._events.Clear();
      EventLoginSyncer.GenerateList();
    }

    public static EventLoginModel getRunningEvent()
    {
      try
      {
        uint num = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
        for (int index = 0; index < EventLoginSyncer._events.Count; ++index)
        {
          EventLoginModel eventLoginModel = EventLoginSyncer._events[index];
          if (eventLoginModel.startDate <= num && num < eventLoginModel.endDate)
            return eventLoginModel;
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return (EventLoginModel) null;
    }
  }
}
