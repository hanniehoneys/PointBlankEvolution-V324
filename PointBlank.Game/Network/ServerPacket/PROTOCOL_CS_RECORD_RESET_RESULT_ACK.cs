using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_RECORD_RESET_RESULT_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 1929);
    }
  }
}
