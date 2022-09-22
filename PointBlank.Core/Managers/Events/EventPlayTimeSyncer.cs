using Npgsql;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Managers.Events
{
  public class EventPlayTimeSyncer
  {
    private static List<PlayTimeModel> _events = new List<PlayTimeModel>();

    public static void GenerateList()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM events_playtime";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            PlayTimeModel playTimeModel = new PlayTimeModel() { _startDate = (uint) npgsqlDataReader.GetInt64(0), _endDate = (uint) npgsqlDataReader.GetInt64(1), _title = npgsqlDataReader.GetString(2), _time = npgsqlDataReader.GetInt64(3), _goodReward1 = npgsqlDataReader.GetInt32(4), _goodReward2 = npgsqlDataReader.GetInt32(5), _goodCount1 = (long) npgsqlDataReader.GetInt32(6), _goodCount2 = (long) npgsqlDataReader.GetInt32(7) };
            EventPlayTimeSyncer._events.Add(playTimeModel);
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
      EventPlayTimeSyncer._events.Clear();
      EventPlayTimeSyncer.GenerateList();
    }

    public static PlayTimeModel getRunningEvent()
    {
      try
      {
        uint num = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
        for (int index = 0; index < EventPlayTimeSyncer._events.Count; ++index)
        {
          PlayTimeModel playTimeModel = EventPlayTimeSyncer._events[index];
          if (playTimeModel._startDate <= num && num < playTimeModel._endDate)
            return playTimeModel;
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return (PlayTimeModel) null;
    }

    public static void ResetPlayerEvent(long pId, PlayerEvent pE)
    {
      if (pId == 0L)
        return;
      ComDiv.updateDB("player_events", "player_id", (object) pId, new string[3]
      {
        "last_playtime_value",
        "last_playtime_finish",
        "last_playtime_date"
      }, (object) pE.LastPlaytimeValue, (object) pE.LastPlaytimeFinish, (object) (long) pE.LastPlaytimeDate);
    }
  }
}
