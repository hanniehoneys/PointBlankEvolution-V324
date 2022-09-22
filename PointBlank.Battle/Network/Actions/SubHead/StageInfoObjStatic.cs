using PointBlank.Battle.Data.Models.SubHead;

namespace PointBlank.Battle.Network.Actions.SubHead
{
  public class StageInfoObjStatic
  {
    public static StageStaticInfo ReadSyncInfo(ReceivePacket p, bool genLog)
    {
      StageStaticInfo stageStaticInfo = new StageStaticInfo() { _isDestroyed = p.readC() };
      if (genLog)
        Logger.warning("[StageInfoObjStatic] Destroyed: " + (object) stageStaticInfo._isDestroyed);
      return stageStaticInfo;
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
    {
      StageStaticInfo stageStaticInfo = StageInfoObjStatic.ReadSyncInfo(p, genLog);
      s.writeC(stageStaticInfo._isDestroyed);
    }
  }
}
