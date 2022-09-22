using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_COMMISSION_MASTER_RESULT_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 1859);
    }
  }
}
