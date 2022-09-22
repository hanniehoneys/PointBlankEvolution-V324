using PointBlank.Battle.Data.Models.Event;
using System;
using System.Collections.Generic;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class CharaFireNHitData
  {
    public static void ReadInfo(ReceivePacket p)
    {
      int num = (int) p.readC();
      p.Advance(17 * num);
    }

    public static List<CharaFireNHitDataInfo> ReadInfo(
      ReceivePacket p,
      bool genLog)
    {
      List<CharaFireNHitDataInfo> fireNhitDataInfoList = new List<CharaFireNHitDataInfo>();
      int num = (int) p.readC();
      for (int index = 0; index < num; ++index)
      {
        CharaFireNHitDataInfo fireNhitDataInfo = new CharaFireNHitDataInfo() { HitInfo = p.readUD(), Extensions = p.readC(), WeaponId = p.readD(), Unk = p.readUH(), X = p.readUH(), Y = p.readUH(), Z = p.readUH() };
        if (genLog)
        {
          Logger.warning("X: " + (object) fireNhitDataInfo.X + " Y: " + (object) fireNhitDataInfo.Y + " Z: " + (object) fireNhitDataInfo.Z);
          Logger.warning("[" + (object) index + "] Hit: " + BitConverter.ToString(p.getBuffer()));
        }
        fireNhitDataInfoList.Add(fireNhitDataInfo);
      }
      return fireNhitDataInfoList;
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
    {
      List<CharaFireNHitDataInfo> fireNhitDataInfoList = CharaFireNHitData.ReadInfo(p, genLog);
      s.writeC((byte) fireNhitDataInfoList.Count);
      for (int index = 0; index < fireNhitDataInfoList.Count; ++index)
      {
        CharaFireNHitDataInfo fireNhitDataInfo = fireNhitDataInfoList[index];
        s.writeD(fireNhitDataInfo.HitInfo);
        s.writeC(fireNhitDataInfo.Extensions);
        s.writeD(fireNhitDataInfo.WeaponId);
        s.writeH(fireNhitDataInfo.Unk);
        s.writeH(fireNhitDataInfo.X);
        s.writeH(fireNhitDataInfo.Y);
        s.writeH(fireNhitDataInfo.Z);
      }
    }
  }
}
