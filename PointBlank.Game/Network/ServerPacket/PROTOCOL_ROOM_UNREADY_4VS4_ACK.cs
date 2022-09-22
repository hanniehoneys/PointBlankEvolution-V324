using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_UNREADY_4VS4_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 3888);
    }
  }
}
