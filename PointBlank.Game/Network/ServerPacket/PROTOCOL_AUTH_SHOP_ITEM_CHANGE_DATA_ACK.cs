using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SHOP_ITEM_CHANGE_DATA_ACK : SendPacket
  {
    private uint Error;

    public PROTOCOL_AUTH_SHOP_ITEM_CHANGE_DATA_ACK(uint Error)
    {
      this.Error = Error;
    }

    public override void write()
    {
      this.writeH((short) 1088);
      this.writeD(this.Error);
    }
  }
}
