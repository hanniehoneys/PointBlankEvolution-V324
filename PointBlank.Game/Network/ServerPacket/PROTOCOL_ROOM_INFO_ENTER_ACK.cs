using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_INFO_ENTER_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 3926);
    }
  }
}
