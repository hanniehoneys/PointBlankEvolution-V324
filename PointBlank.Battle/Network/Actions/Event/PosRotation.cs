using PointBlank.Battle.Data.Models.Event;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class PosRotation
  {
    public static PosRotationInfo ReadInfo(ReceivePacket p, bool genLog)
    {
      PosRotationInfo posRotationInfo = new PosRotationInfo() { RotationX = p.readUH(), RotationY = p.readUH(), RotationZ = p.readUH(), CameraX = p.readUH(), CameraY = p.readUH(), Area = p.readUH() };
      if (!genLog)
        ;
      return posRotationInfo;
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
    {
      PosRotationInfo info = PosRotation.ReadInfo(p, genLog);
      PosRotation.WriteInfo(s, info);
    }

    public static void WriteInfo(SendPacket s, PosRotationInfo info)
    {
      s.writeH(info.RotationX);
      s.writeH(info.RotationY);
      s.writeH(info.RotationZ);
      s.writeH(info.CameraX);
      s.writeH(info.CameraY);
      s.writeH(info.Area);
    }
  }
}
