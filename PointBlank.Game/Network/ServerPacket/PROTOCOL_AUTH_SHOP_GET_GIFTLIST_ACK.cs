using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SHOP_GET_GIFTLIST_ACK : SendPacket
  {
    private uint Error;

    public PROTOCOL_AUTH_SHOP_GET_GIFTLIST_ACK(uint Error)
    {
      this.Error = Error;
    }

    public override void write()
    {
      this.writeH((short) 1042);
      this.writeH((short) 0);
      if (this.Error == 0U)
        return;
      this.writeD(this.Error);
    }
  }
}
