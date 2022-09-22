using Npgsql;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Managers.Events
{
  public class EventMapSyncer
  {
    private static List<EventMapModel> _events = new List<EventMapModel>();

    public static void GenerateList()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM events_mapbonus";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            EventMapModel eventMapModel = new EventMapModel() { _startDate = (uint) npgsqlDataReader.GetInt64(0), _endDate = (uint) npgsqlDataReader.GetInt64(1), _mapId = npgsqlDataReader.GetInt32(2), _stageType = npgsqlDataReader.GetInt32(3), _percentXp = npgsqlDataReader.GetInt32(4), _percentGp = npgsqlDataReader.GetInt32(5) };
            EventMapSyncer._events.Add(eventMapModel);
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
      EventMapSyncer._events.Clear();
      EventMapSyncer.GenerateList();
    }

    public static EventMapModel getRunningEvent()
    {
      try
      {
        uint num = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
        for (int index = 0; index < EventMapSyncer._events.Count; ++index)
        {
          EventMapModel eventMapModel = EventMapSyncer._events[index];
          if (eventMapModel._startDate <= num && num < eventMapModel._endDate)
            return eventMapModel;
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return (EventMapModel) null;
    }

    public static bool EventIsValid(EventMapModel ev, int map, int stageType)
    {
      return ev != null && (ev._mapId == map || ev._stageType == stageType);
    }
  }
}
