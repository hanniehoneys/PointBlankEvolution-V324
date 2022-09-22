using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_GET_USER_BASIC_INFO_ACK : SendPacket
  {
    private uint Error;

    public PROTOCOL_BASE_GET_USER_BASIC_INFO_ACK(uint Error)
    {
      this.Error = Error;
    }

    public override void write()
    {
      this.writeD(this.Error);
    }
  }
}
