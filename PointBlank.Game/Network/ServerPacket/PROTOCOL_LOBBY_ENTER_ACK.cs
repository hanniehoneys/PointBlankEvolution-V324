using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_LOBBY_ENTER_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 3074);
      this.writeH((short) 0);
      this.writeD(0);
      this.writeC((byte) 1);
      this.writeQ(0L);
    }
  }
}
