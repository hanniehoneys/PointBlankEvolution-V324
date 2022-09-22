
using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_URL_LIST_ACK : SendPacket
  {
    public override void write()
    {
      string name1 = "http://pb-phoenix.com/";
      string name2 = "https://www.facebook.com/groups/2038630256436318/";
      this.writeH((short) 673);
      this.writeC((byte) 1);
      this.writeC((byte) 2);
      this.writeH((ushort) name1.Length);
      this.writeQ(4L);
      this.writeText(name1, name1.Length);
      this.writeH((ushort) name2.Length);
      this.writeQ(3L);
      this.writeText(name2, name2.Length);
    }
  }
}
