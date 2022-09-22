using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SHOP_USE_GIFTCOUPON_ACK : SendPacket
  {
    private uint Error;

    public PROTOCOL_AUTH_SHOP_USE_GIFTCOUPON_ACK(uint Error)
    {
      this.Error = Error;
    }

    public override void write()
    {
      this.writeH((short) 1085);
      this.writeD(this.Error);
    }
  }
}
