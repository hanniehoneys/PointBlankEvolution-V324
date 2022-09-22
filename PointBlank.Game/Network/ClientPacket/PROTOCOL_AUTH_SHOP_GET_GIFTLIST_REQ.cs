using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_SHOP_GET_GIFTLIST_REQ : ReceivePacket
  {
    public PROTOCOL_AUTH_SHOP_GET_GIFTLIST_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
    }

    public override void run()
    {
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GET_GIFTLIST_ACK(2148110592U));
      }
      catch
      {
      }
    }
  }
}
