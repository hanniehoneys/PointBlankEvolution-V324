using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_ROOM_INVITED_ACK : SendPacket
  {
    private int Error;

    public PROTOCOL_CS_ROOM_INVITED_ACK(int Error)
    {
      this.Error = Error;
    }

    public override void write()
    {
      this.writeH((short) 1902);
      this.writeD(this.Error);
    }
  }
}
