using PointBlank.Core.Managers;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK : SendPacket
  {
    private int _tudo;
    private ShopData data;

    public PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK(ShopData data, int tudo)
    {
      this.data = data;
      this._tudo = tudo;
    }

    public override void write()
    {
      this.writeH((short) 1040);
      this.writeD(this._tudo);
      this.writeD(this.data.ItemsCount);
      this.writeD(this.data.Offset);
      this.writeB(this.data.Buffer);
      this.writeD(585);
    }
  }
}
