using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Models.Event;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class GetWeaponForHost
  {
    public static WeaponHost ReadInfo(ActionModel ac, ReceivePacket p, bool genLog)
    {
      WeaponHost weaponHost = new WeaponHost() { DeathType = p.readC(), HitPart = p.readC(), X = p.readUH(), Y = p.readUH(), Z = p.readUH(), WeaponId = p.readD() };
      if (genLog)
        Logger.warning("Slot: " + (object) ac.Slot + " Type: " + (object) weaponHost.DeathType + " Hit: " + (object) weaponHost.HitPart + " X: " + (object) weaponHost.X + " Y: " + (object) weaponHost.Y + " Z: " + (object) weaponHost.Z + " WeaponId: " + (object) weaponHost.WeaponId);
      return weaponHost;
    }

    public static void ReadInfo(ReceivePacket p)
    {
      p.Advance(13);
    }

    public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
    {
      WeaponHost info = GetWeaponForHost.ReadInfo(ac, p, genLog);
      GetWeaponForHost.WriteInfo(s, info);
    }

    public static void WriteInfo(SendPacket s, WeaponHost info)
    {
      s.writeC(info.DeathType);
      s.writeC(info.HitPart);
      s.writeH(info.X);
      s.writeH(info.Y);
      s.writeH(info.Z);
      s.writeD(info.WeaponId);
    }
  }
}
