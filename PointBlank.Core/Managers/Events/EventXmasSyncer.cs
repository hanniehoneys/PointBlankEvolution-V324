using Npgsql;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Managers.Events
{
  public class EventXmasSyncer
  {
    private static List<EventXmasModel> _events = new List<EventXmasModel>();

    public static void GenerateList()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM events_xmas";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            EventXmasModel eventXmasModel = new EventXmasModel() { startDate = (uint) npgsqlDataReader.GetInt64(0), endDate = (uint) npgsqlDataReader.GetInt64(1) };
            EventXmasSyncer._events.Add(eventXmasModel);
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
      EventXmasSyncer._events.Clear();
      EventXmasSyncer.GenerateList();
    }

    public static EventXmasModel getRunningEvent()
    {
      try
      {
        uint num = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
        for (int index = 0; index < EventXmasSyncer._events.Count; ++index)
        {
          EventXmasModel eventXmasModel = EventXmasSyncer._events[index];
          if (eventXmasModel.startDate <= num && num < eventXmasModel.endDate)
            return eventXmasModel;
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return (EventXmasModel) null;
    }
  }
}
