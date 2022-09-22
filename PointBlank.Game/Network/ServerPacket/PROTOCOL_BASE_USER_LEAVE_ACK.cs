using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_USER_LEAVE_ACK : SendPacket
  {
    private int error;

    public PROTOCOL_BASE_USER_LEAVE_ACK(int error)
    {
      this.error = error;
    }

    public override void write()
    {
      this.writeH((short) 537);
      this.writeD(this.error);
    }
  }
}
