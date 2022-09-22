using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Shop;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CHAR_CREATE_CHARA_REQ : ReceivePacket
  {
    private List<CartGoods> ShopCart = new List<CartGoods>();
    private string Name;

    public PROTOCOL_CHAR_CREATE_CHARA_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.Name = this.readUnicode((int) this.readC() * 2);
      int num = (int) this.readC();
      this.ShopCart.Add(new CartGoods()
      {
        GoodId = this.readD(),
        BuyType = (int) this.readC()
      });
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null || player.player_name.Length == 0)
          this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CREATE_CHARA_ACK(2147487767U, 0, (Character) null, (PointBlank.Game.Data.Model.Account) null));
        else if (player._inventory._items.Count >= 500 || player.Characters.Count >= 16)
        {
          this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CREATE_CHARA_ACK(2147487929U, 0, (Character) null, (PointBlank.Game.Data.Model.Account) null));
        }
        else
        {
          int GoldPrice;
          int CashPrice;
          List<GoodItem> goods = ShopManager.getGoods(this.ShopCart, out GoldPrice, out CashPrice);
          if (goods.Count == 0)
            this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CREATE_CHARA_ACK(2147487767U, 0, (Character) null, (PointBlank.Game.Data.Model.Account) null));
          else if (0 > player._gp - GoldPrice || 0 > player._money - CashPrice)
            this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CREATE_CHARA_ACK(2147487768U, 0, (Character) null, (PointBlank.Game.Data.Model.Account) null));
          else if (PlayerManager.updateAccountCashing(player.player_id, player._gp - GoldPrice, player._money - CashPrice))
          {
            player._gp -= GoldPrice;
            player._money -= CashPrice;
            int count = player.Characters.Count;
            Character Model = new Character();
            for (int index = 0; index < goods.Count; ++index)
            {
              GoodItem goodItem = goods[index];
              Model.Id = goodItem._item._id;
              Model.Name = this.Name;
              Model.PlayTime = 0;
              Model.Slot = count++;
              Model.CreateDate = int.Parse(DateTime.Now.ToString("yyMMddHHmm"));
              if (player.Characters.Find((Predicate<Character>) (x => x.Id == Model.Id)) == null)
              {
                CharacterManager.Create(Model, player.player_id);
                player.Characters.Add(Model);
              }
            }
            this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, goods));
            this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CREATE_CHARA_ACK(0U, 1, Model, player));
          }
          else
            this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CREATE_CHARA_ACK(2147487769U, 0, (Character) null, (PointBlank.Game.Data.Model.Account) null));
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }
  }
}
