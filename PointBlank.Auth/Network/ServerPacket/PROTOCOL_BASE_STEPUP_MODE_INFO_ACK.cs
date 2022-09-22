
using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_STEPUP_MODE_INFO_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 691);
      this.writeD(0);
    }
  }
}
