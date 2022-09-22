using PointBlank.Battle.Data.Models.Event;

namespace PointBlank.Battle.Network.Actions.Event
{
  public class FireDataOnObject
  {
    public static FireDataObjectInfo ReadInfo(ReceivePacket p, bool genLog)
    {
      return new FireDataObjectInfo() { ShotId = p.readUH() };
    }

    public static void ReadInfo(ReceivePacket p)
    {
      p.Advance(2);
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
    {
      FireDataObjectInfo fireDataObjectInfo = FireDataOnObject.ReadInfo(p, genLog);
      s.writeH(fireDataObjectInfo.ShotId);
    }
  }
}
