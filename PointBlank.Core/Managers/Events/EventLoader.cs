namespace PointBlank.Core.Managers.Events
{
  public static class EventLoader
  {
    public static void LoadAll()
    {
      EventVisitSyncer.GenerateList();
      EventLoginSyncer.GenerateList();
      EventMapSyncer.GenerateList();
      EventPlayTimeSyncer.GenerateList();
      EventQuestSyncer.GenerateList();
      EventRankUpSyncer.GenerateList();
      EventXmasSyncer.GenerateList();
    }

    public static void ReloadEvent(int index)
    {
      switch (index)
      {
        case 0:
          EventVisitSyncer.ReGenList();
          break;
        case 1:
          EventLoginSyncer.ReGenList();
          break;
        case 2:
          EventMapSyncer.ReGenList();
          break;
        case 3:
          EventPlayTimeSyncer.ReGenList();
          break;
        case 4:
          EventQuestSyncer.ReGenList();
          break;
        case 5:
          EventRankUpSyncer.ReGenList();
          break;
        case 6:
          EventXmasSyncer.ReGenList();
          break;
      }
    }

    public static void ReloadAll()
    {
      EventVisitSyncer.ReGenList();
      EventLoginSyncer.ReGenList();
      EventMapSyncer.ReGenList();
      EventPlayTimeSyncer.ReGenList();
      EventQuestSyncer.ReGenList();
      EventRankUpSyncer.ReGenList();
      EventXmasSyncer.ReGenList();
    }
  }
}
