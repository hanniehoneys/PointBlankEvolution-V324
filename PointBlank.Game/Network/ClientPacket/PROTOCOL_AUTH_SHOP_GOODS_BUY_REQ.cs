using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Shop;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_SHOP_GOODS_BUY_REQ : ReceivePacket
  {
    private List<CartGoods> ShopCart = new List<CartGoods>();

    public PROTOCOL_AUTH_SHOP_GOODS_BUY_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      int num = (int) this.readC();
      for (int index = 0; index < num; ++index)
      {
        this.ShopCart.Add(new CartGoods()
        {
          GoodId = this.readD(),
          BuyType = (int) this.readC()
        });
        this.readQ();
      }
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null || player.player_name.Length == 0)
          this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GOODS_BUY_ACK(2147487767U, (List<GoodItem>) null, (Account) null));
        else if (player._inventory._items.Count >= 500)
        {
          this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GOODS_BUY_ACK(2147487929U, (List<GoodItem>) null, (Account) null));
        }
        else
        {
          int GoldPrice;
          int CashPrice;
          List<GoodItem> goods = ShopManager.getGoods(this.ShopCart, out GoldPrice, out CashPrice);
          if (goods.Count == 0)
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GOODS_BUY_ACK(2147487767U, (List<GoodItem>) null, (Account) null));
          else if (0 > player._gp - GoldPrice || 0 > player._money - CashPrice)
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GOODS_BUY_ACK(2147487768U, (List<GoodItem>) null, (Account) null));
          else if (PlayerManager.updateAccountCashing(player.player_id, player._gp - GoldPrice, player._money - CashPrice))
          {
            player._gp -= GoldPrice;
            player._money -= CashPrice;
            this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, goods));
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GOODS_BUY_ACK(1U, goods, player));
          }
          else
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GOODS_BUY_ACK(2147487769U, (List<GoodItem>) null, (Account) null));
        }
      }
      catch (Exception ex)
      {
        Logger.error("PROTOCOL_AUTH_SHOP_GOODS_BUY_REQ: " + ex.ToString());
      }
    }
  }
}
