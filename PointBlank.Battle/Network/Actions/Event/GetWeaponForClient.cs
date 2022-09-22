using PointBlank.Battle.Data.Configs;
using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Models.Event;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class GetWeaponForClient
  {
    public static WeaponClient ReadInfo(ActionModel ac, ReceivePacket p, bool genLog)
    {
      WeaponClient weaponClient = new WeaponClient() { WeaponFlag = p.readC(), WeaponId = p.readD(), Extensions = p.readC(), AmmoPrin = p.readUH(), AmmoDual = p.readUH(), AmmoTotal = p.readUH(), Unk1 = p.readUH(), Unk2 = p.readD() };
      if (genLog)
        Logger.warning("Slot: " + (object) ac.Slot + " WeaponId: " + (object) weaponClient.WeaponId);
      return weaponClient;
    }

    public static void ReadInfo(ReceivePacket p)
    {
      p.Advance(8);
    }

    public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
    {
      WeaponClient weaponClient = GetWeaponForClient.ReadInfo(ac, p, genLog);
      s.writeC(weaponClient.WeaponFlag);
      s.writeD(weaponClient.WeaponId);
      s.writeC(weaponClient.Extensions);
      if (BattleConfig.useMaxAmmoInDrop)
      {
        s.writeH(ushort.MaxValue);
        s.writeH(weaponClient.AmmoDual);
        s.writeH((short) 10000);
      }
      else
      {
        s.writeH(weaponClient.AmmoPrin);
        s.writeH(weaponClient.AmmoDual);
        s.writeH(weaponClient.AmmoTotal);
      }
      s.writeH(weaponClient.Unk1);
      s.writeD(weaponClient.Unk2);
    }
  }
}
