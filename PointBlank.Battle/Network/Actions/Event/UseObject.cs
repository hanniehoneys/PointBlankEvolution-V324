using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Models.Event;
using System.Collections.Generic;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class UseObject
  {
    public static List<UseObjectInfo> ReadSyncInfo(
      ActionModel ac,
      ReceivePacket p,
      bool genLog)
    {
      List<UseObjectInfo> useObjectInfoList = new List<UseObjectInfo>();
      int num = (int) p.readC();
      for (int index = 0; index < num; ++index)
      {
        UseObjectInfo useObjectInfo = new UseObjectInfo() { Use = p.readC(), SpaceFlags = (CHARA_MOVES) p.readC(), ObjectId = p.readUH() };
        if (genLog)
          Logger.warning("Slot: " + (object) ac.Slot + " UseObject: Flag: " + (object) useObjectInfo.SpaceFlags + " ObjectId: " + (object) useObjectInfo.ObjectId);
        useObjectInfoList.Add(useObjectInfo);
      }
      return useObjectInfoList;
    }

    public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
    {
      List<UseObjectInfo> Infos = UseObject.ReadSyncInfo(ac, p, genLog);
      UseObject.WriteInfo(s, Infos);
    }

    public static void WriteInfo(SendPacket s, List<UseObjectInfo> Infos)
    {
      s.writeC((byte) Infos.Count);
      for (int index = 0; index < Infos.Count; ++index)
      {
        UseObjectInfo info = Infos[index];
        s.writeC(info.Use);
        s.writeC((byte) info.SpaceFlags);
        s.writeH(info.ObjectId);
      }
    }
  }
}
