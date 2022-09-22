using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_LOGOUT_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 516);
    }
  }
}
