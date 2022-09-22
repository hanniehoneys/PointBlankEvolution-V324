using PointBlank.Battle.Data.Models.Event;
using System.Collections.Generic;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class Suicide
  {
    public static List<SuicideInfo> ReadInfo(
      ReceivePacket p,
      bool genLog,
      bool OnlyBytes = false)
    {
      return Suicide.BaseReadInfo(p, OnlyBytes, genLog);
    }

    private static List<SuicideInfo> BaseReadInfo(
      ReceivePacket p,
      bool OnlyBytes,
      bool genLog)
    {
      List<SuicideInfo> suicideInfoList = new List<SuicideInfo>();
      int num = (int) p.readC();
      for (int index = 0; index < num; ++index)
      {
        SuicideInfo suicideInfo = new SuicideInfo() { HitInfo = p.readUD(), Extensions = p.readC(), WeaponId = p.readD(), PlayerPos = p.readUHVector() };
        if (OnlyBytes)
          ;
        if (genLog)
          Logger.warning("[" + (object) index + "] Suicide: Hit: " + (object) suicideInfo.HitInfo + " WeaponId: " + (object) suicideInfo.WeaponId + " X: " + (object) suicideInfo.PlayerPos.X + " Y: " + (object) suicideInfo.PlayerPos.Y + " Z: " + (object) suicideInfo.PlayerPos.Z);
        suicideInfoList.Add(suicideInfo);
      }
      return suicideInfoList;
    }

    public static void ReadInfo(ReceivePacket p)
    {
      int num = (int) p.readC();
      p.Advance(15 * num);
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
    {
      List<SuicideInfo> hits = Suicide.ReadInfo(p, genLog, true);
      Suicide.WriteInfo(s, hits);
    }

    public static void WriteInfo(SendPacket s, List<SuicideInfo> hits)
    {
      s.writeC((byte) hits.Count);
      for (int index = 0; index < hits.Count; ++index)
      {
        SuicideInfo hit = hits[index];
        s.writeD(hit.HitInfo);
        s.writeC(hit.Extensions);
        s.writeD(hit.WeaponId);
        s.writeHVector(hit.PlayerPos);
      }
    }
  }
}
