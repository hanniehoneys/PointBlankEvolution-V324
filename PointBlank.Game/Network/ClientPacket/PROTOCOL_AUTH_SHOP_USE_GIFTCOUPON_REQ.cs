using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_SHOP_USE_GIFTCOUPON_REQ : ReceivePacket
  {
    private string Token;

    public PROTOCOL_AUTH_SHOP_USE_GIFTCOUPON_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.Token = this.readS((int) this.readC());
    }

    public override void run()
    {
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK("ระบบนี้ยังไม่เปิดใช้งาน"));
      }
      catch (Exception ex)
      {
        Logger.error("PROTOCOL_AUTH_SHOP_USE_GIFTCOUPON_REQ: " + ex.ToString());
      }
    }
  }
}
