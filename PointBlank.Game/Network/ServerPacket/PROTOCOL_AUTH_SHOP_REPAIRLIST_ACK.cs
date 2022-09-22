using PointBlank.Core.Managers;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SHOP_REPAIRLIST_ACK : SendPacket
  {
    private int Total;
    private ShopData Data;

    public PROTOCOL_AUTH_SHOP_REPAIRLIST_ACK(ShopData Data, int Total)
    {
      this.Data = Data;
      this.Total = Total;
    }

    public override void write()
    {
      this.writeH((short) 1070);
      this.writeD(this.Total);
      this.writeD(this.Data.ItemsCount);
      this.writeD(this.Data.Offset);
      this.writeB(this.Data.Buffer);
      this.writeD(585);
    }
  }
}
