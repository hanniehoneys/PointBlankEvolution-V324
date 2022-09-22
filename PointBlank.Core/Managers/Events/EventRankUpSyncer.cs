using Npgsql;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Managers.Events
{
  public class EventRankUpSyncer
  {
    private static List<EventUpModel> _events = new List<EventUpModel>();

    public static void GenerateList()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM events_rankup";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            EventUpModel eventUpModel = new EventUpModel() { _startDate = (uint) npgsqlDataReader.GetInt64(0), _endDate = (uint) npgsqlDataReader.GetInt64(1), _percentXp = npgsqlDataReader.GetInt32(2), _percentGp = npgsqlDataReader.GetInt32(3) };
            EventRankUpSyncer._events.Add(eventUpModel);
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
      EventRankUpSyncer._events.Clear();
      EventRankUpSyncer.GenerateList();
    }

    public static EventUpModel getRunningEvent()
    {
      try
      {
        uint num = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
        for (int index = 0; index < EventRankUpSyncer._events.Count; ++index)
        {
          EventUpModel eventUpModel = EventRankUpSyncer._events[index];
          if (eventUpModel._startDate <= num && num < eventUpModel._endDate)
            return eventUpModel;
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return (EventUpModel) null;
    }
  }
}
