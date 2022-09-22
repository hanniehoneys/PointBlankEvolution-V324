using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_LOBBY_QUICKJOIN_ROOM_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 5378);
    }
  }
}
