using Npgsql;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace PointBlank.Core.Managers.Events
{
  public class EventQuestSyncer
  {
    private static List<QuestModel> _events = new List<QuestModel>();

    public static void GenerateList()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          npgsqlConnection.Open();
          command.CommandText = "SELECT * FROM events_quest";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            QuestModel questModel = new QuestModel() { startDate = (uint) npgsqlDataReader.GetInt64(0), endDate = (uint) npgsqlDataReader.GetInt64(1) };
            EventQuestSyncer._events.Add(questModel);
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
      EventQuestSyncer._events.Clear();
      EventQuestSyncer.GenerateList();
    }

    public static QuestModel getRunningEvent()
    {
      try
      {
        uint num = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
        for (int index = 0; index < EventQuestSyncer._events.Count; ++index)
        {
          QuestModel questModel = EventQuestSyncer._events[index];
          if (questModel.startDate <= num && num < questModel.endDate)
            return questModel;
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
      return (QuestModel) null;
    }

    public static void ResetPlayerEvent(long pId, PlayerEvent pE)
    {
      if (pId == 0L)
        return;
      ComDiv.updateDB("player_events", "player_id", (object) pId, new string[2]
      {
        "last_quest_date",
        "last_quest_finish"
      }, (object) (long) pE.LastQuestDate, (object) pE.LastQuestFinish);
    }
  }
}
