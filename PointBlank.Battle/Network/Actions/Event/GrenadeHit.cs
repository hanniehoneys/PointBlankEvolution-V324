using PointBlank.Battle.Data;
using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Models.Event;
using System.Collections.Generic;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class GrenadeHit
  {
    public static List<GrenadeHitInfo> ReadInfo(
      ReceivePacket p,
      bool genLog,
      bool OnlyBytes = false)
    {
      return GrenadeHit.BaseReadInfo(p, OnlyBytes, genLog);
    }

    public static void ReadInfo(ReceivePacket p)
    {
      int num = (int) p.readC();
      p.Advance(32 * num);
    }

    private static List<GrenadeHitInfo> BaseReadInfo(
      ReceivePacket p,
      bool OnlyBytes,
      bool genLog)
    {
      List<GrenadeHitInfo> grenadeHitInfoList = new List<GrenadeHitInfo>();
      int num1 = (int) p.readC();
      for (int index1 = 0; index1 < num1; ++index1)
      {
        GrenadeHitInfo grenadeHitInfo = new GrenadeHitInfo() { HitInfo = p.readUD(), BoomInfo = p.readUH(), PlayerPos = p.readUHVector(), Extensions = p.readC(), WeaponId = p.readD(), DeathType = p.readC(), FirePos = p.readUHVector(), HitPos = p.readUHVector(), GrenadesCount = p.readUH() };
        if (!OnlyBytes)
        {
          grenadeHitInfo.HitEnum = (HIT_TYPE) AllUtils.getHitHelmet(grenadeHitInfo.HitInfo);
          if (grenadeHitInfo.BoomInfo > (ushort) 0)
          {
            grenadeHitInfo.BoomPlayers = new List<int>();
            for (int index2 = 0; index2 < 16; ++index2)
            {
              int num2 = 1 << index2;
              if (((int) grenadeHitInfo.BoomInfo & num2) == num2)
                grenadeHitInfo.BoomPlayers.Add(index2);
            }
          }
          grenadeHitInfo.WeaponClass = (CLASS_TYPE) AllUtils.getIdStatics(grenadeHitInfo.WeaponId, 2);
        }
        if (genLog)
        {
          Logger.warning("[Player Postion] X: " + (object) grenadeHitInfo.FirePos.X + "; Y: " + (object) grenadeHitInfo.FirePos.Y + "; Z: " + (object) grenadeHitInfo.FirePos.Z);
          Logger.warning("[Object Postion] X: " + (object) grenadeHitInfo.HitPos.X + "; Y: " + (object) grenadeHitInfo.HitPos.Y + "; Z: " + (object) grenadeHitInfo.HitPos.Z);
        }
        grenadeHitInfoList.Add(grenadeHitInfo);
      }
      return grenadeHitInfoList;
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
    {
      List<GrenadeHitInfo> hits = GrenadeHit.ReadInfo(p, genLog, true);
      GrenadeHit.WriteInfo(s, hits);
    }

    public static void WriteInfo(SendPacket s, List<GrenadeHitInfo> hits)
    {
      s.writeC((byte) hits.Count);
      for (int index = 0; index < hits.Count; ++index)
      {
        GrenadeHitInfo hit = hits[index];
        s.writeD(hit.HitInfo);
        s.writeH(hit.BoomInfo);
        s.writeHVector(hit.PlayerPos);
        s.writeC(hit.Extensions);
        s.writeD(hit.WeaponId);
        s.writeC(hit.DeathType);
        s.writeHVector(hit.FirePos);
        s.writeHVector(hit.HitPos);
        s.writeH(hit.GrenadesCount);
      }
    }
  }
}
