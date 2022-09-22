using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CLIENT_LEAVE_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 1796);
    }
  }
}
