using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_BOOSTEVENT_INFO_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 676);
      this.writeQ(1L);
      this.writeC((byte) 0);
    }
  }
}
