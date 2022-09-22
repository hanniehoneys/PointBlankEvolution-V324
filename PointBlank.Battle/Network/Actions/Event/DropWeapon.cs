using PointBlank.Battle.Data.Configs;
using PointBlank.Battle.Data.Models.Event;
using System;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class DropWeapon
  {
    public static DropWeaponInfo ReadInfo(ReceivePacket p, bool genLog)
    {
      DropWeaponInfo dropWeaponInfo = new DropWeaponInfo() { WeaponFlag = p.readC(), WeaponId = p.readD(), Extensions = p.readC(), AmmoPrin = p.readUH(), AmmoDual = p.readUH(), AmmoTotal = p.readUH(), Unk1 = p.readUH(), Unk2 = p.readD() };
      if (genLog)
      {
        Logger.warning("[ActionBuffer]: " + BitConverter.ToString(p.getBuffer()));
        Logger.warning("[DropWeapon] WeaponId: " + (object) dropWeaponInfo.WeaponId);
      }
      return dropWeaponInfo;
    }

    public static void ReadInfo(ReceivePacket p)
    {
      p.Advance(8);
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog, int count)
    {
      DropWeaponInfo dropWeaponInfo = DropWeapon.ReadInfo(p, genLog);
      s.writeC((byte) ((uint) dropWeaponInfo.WeaponFlag + (uint) count));
      s.writeD(dropWeaponInfo.WeaponId);
      s.writeC(dropWeaponInfo.Extensions);
      if (BattleConfig.useMaxAmmoInDrop)
      {
        s.writeH(ushort.MaxValue);
        s.writeH(dropWeaponInfo.AmmoDual);
        s.writeH((short) 10000);
      }
      else
      {
        s.writeH(dropWeaponInfo.AmmoPrin);
        s.writeH(dropWeaponInfo.AmmoDual);
        s.writeH(dropWeaponInfo.AmmoTotal);
      }
      s.writeH(dropWeaponInfo.Unk1);
      s.writeD(dropWeaponInfo.Unk2);
    }
  }
}
