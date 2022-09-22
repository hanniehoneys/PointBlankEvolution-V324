using PointBlank.Battle.Data.Models.SubHead;

namespace PointBlank.Battle.Network.Actions.SubHead
{
  public class ObjectStatic
  {
    public static byte[] ReadInfo(ReceivePacket p)
    {
      return p.readB(10);
    }

    public static ObjectStaticInfo ReadInfo(ReceivePacket p, bool genLog)
    {
      ObjectStaticInfo objectStaticInfo = new ObjectStaticInfo() { Type = p.readUH(), Life = p.readUH(), DestroyedBySlot = p.readUH(), Unk = p.readD() };
      if (genLog)
        Logger.warning("[ObjectStatic] Life: " + (object) objectStaticInfo.Life + " Destroyed: " + (object) objectStaticInfo.DestroyedBySlot);
      return objectStaticInfo;
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p)
    {
      s.writeB(ObjectStatic.ReadInfo(p));
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
    {
      ObjectStaticInfo objectStaticInfo = ObjectStatic.ReadInfo(p, genLog);
      s.writeH(objectStaticInfo.Type);
      s.writeH(objectStaticInfo.Life);
      s.writeH(objectStaticInfo.DestroyedBySlot);
      s.writeD(objectStaticInfo.Unk);
    }
  }
}
