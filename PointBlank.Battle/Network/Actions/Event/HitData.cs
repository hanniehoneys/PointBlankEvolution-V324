using PointBlank.Battle.Data;
using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Models.Event;
using System.Collections.Generic;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class HitData
  {
    public static List<HitDataInfo> ReadInfo(
      ReceivePacket Packet,
      bool Log,
      bool OnlyBytes = false)
    {
      return HitData.BaseReadInfo(Packet, OnlyBytes, Log);
    }

    public static void ReadInfo(ReceivePacket Packet)
    {
      int num = (int) Packet.readC();
      Packet.Advance(35 * num);
    }

    private static List<HitDataInfo> BaseReadInfo(
      ReceivePacket Packet,
      bool OnlyBytes,
      bool Log)
    {
      List<HitDataInfo> hitDataInfoList = new List<HitDataInfo>();
      int num1 = (int) Packet.readC();
      for (int index1 = 0; index1 < num1; ++index1)
      {
        HitDataInfo hitDataInfo = new HitDataInfo() { HitIndex = Packet.readUD(), BoomInfo = Packet.readUH(), Extensions = Packet.readC(), WeaponId = Packet.readD(), StartBullet = Packet.readTVector(), EndBullet = Packet.readTVector() };
        if (!OnlyBytes)
        {
          hitDataInfo.HitEnum = (HIT_TYPE) AllUtils.getHitHelmet(hitDataInfo.HitIndex);
          if (hitDataInfo.BoomInfo > (ushort) 0)
          {
            hitDataInfo.BoomPlayers = new List<int>();
            for (int index2 = 0; index2 < 16; ++index2)
            {
              int num2 = 1 << index2;
              if (((int) hitDataInfo.BoomInfo & num2) == num2)
                hitDataInfo.BoomPlayers.Add(index2);
            }
          }
          hitDataInfo.WeaponClass = (CLASS_TYPE) AllUtils.getIdStatics(hitDataInfo.WeaponId, 2);
        }
        if (!Log);
        hitDataInfoList.Add(hitDataInfo);
      }
      return hitDataInfoList;
    }

    public static void WriteInfo(SendPacket Send, ReceivePacket Packet, bool Log)
    {
      List<HitDataInfo> Hits = HitData.ReadInfo(Packet, Log, true);
      HitData.WriteInfo(Send, Hits);
    }

    public static void WriteInfo(SendPacket Send, List<HitDataInfo> Hits)
    {
      Send.writeC((byte) Hits.Count);
      for (int index = 0; index < Hits.Count; ++index)
      {
        HitDataInfo hit = Hits[index];
        Send.writeD(hit.HitIndex);
        Send.writeH(hit.BoomInfo);
        Send.writeC(hit.Extensions);
        Send.writeD(hit.WeaponId);
        Send.writeTVector(hit.StartBullet);
        Send.writeTVector(hit.EndBullet);
      }
    }
  }
}
