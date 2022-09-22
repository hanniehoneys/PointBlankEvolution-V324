using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SERVER_MESSAGE_KICK_PLAYER_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 2563);
      this.writeC((byte) 0);
    }
  }
}
