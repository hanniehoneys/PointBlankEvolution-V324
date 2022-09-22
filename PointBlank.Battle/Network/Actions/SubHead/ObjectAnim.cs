using PointBlank.Battle.Data.Models.SubHead;

namespace PointBlank.Battle.Network.Actions.SubHead
{
  public class ObjectAnim
  {
    public static byte[] ReadInfo(ReceivePacket p)
    {
      return p.readB(8);
    }

    public static ObjectAnimInfo ReadInfo(ReceivePacket p, bool genLog)
    {
      ObjectAnimInfo objectAnimInfo = new ObjectAnimInfo() { _life = p.readUH(), _anim1 = p.readC(), _anim2 = p.readC(), _syncDate = p.readT() };
      if (genLog)
        Logger.warning("[ObjectAnim] Life: " + (object) objectAnimInfo._life + " Animation[1]: " + (object) objectAnimInfo._anim1 + " Animation[2]: " + (object) objectAnimInfo._anim2 + " Sync: " + (object) objectAnimInfo._syncDate);
      return objectAnimInfo;
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p)
    {
      s.writeB(ObjectAnim.ReadInfo(p));
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
    {
      ObjectAnimInfo objectAnimInfo = ObjectAnim.ReadInfo(p, genLog);
      s.writeH(objectAnimInfo._life);
      s.writeC(objectAnimInfo._anim1);
      s.writeC(objectAnimInfo._anim2);
      s.writeT(objectAnimInfo._syncDate);
    }
  }
}
