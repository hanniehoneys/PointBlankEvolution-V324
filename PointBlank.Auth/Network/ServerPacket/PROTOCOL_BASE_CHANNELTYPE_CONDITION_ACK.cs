using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_CHANNELTYPE_CONDITION_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 693);
      this.writeB(new byte[888]);
    }
  }
}
