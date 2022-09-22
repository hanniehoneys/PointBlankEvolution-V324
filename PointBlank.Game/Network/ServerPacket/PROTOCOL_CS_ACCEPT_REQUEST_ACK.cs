using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_ACCEPT_REQUEST_ACK : SendPacket
  {
    private uint result;

    public PROTOCOL_CS_ACCEPT_REQUEST_ACK(uint result)
    {
      this.result = result;
    }

    public override void write()
    {
      this.writeH((short) 1847);
      this.writeD(this.result);
    }
  }
}
