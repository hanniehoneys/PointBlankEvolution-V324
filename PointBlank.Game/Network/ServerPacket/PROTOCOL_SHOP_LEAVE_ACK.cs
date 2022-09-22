using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SHOP_LEAVE_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 1028);
      this.writeH((short) 0);
      this.writeD(0);
    }
  }
}
