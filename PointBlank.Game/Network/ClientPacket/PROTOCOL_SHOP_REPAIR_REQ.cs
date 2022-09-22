using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Shop;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_SHOP_REPAIR_REQ : ReceivePacket
  {
    private uint Error = 1;
    private long ObjectId;

    public PROTOCOL_SHOP_REPAIR_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      int num = (int) this.readH();
      this.ObjectId = (long) this.readD();
    }

    public override void run()
    {
      if (this._client == null)
        return;
      if (this._client._player == null)
        return;
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        ItemsModel Item = player._inventory.getItem(this.ObjectId);
        if (Item == null)
          return;
        ItemRepair itemRepair = ShopManager.ItemRepairs.Find((Predicate<ItemRepair>) (x => x.ItemId == Item._id));
        if (itemRepair != null)
        {
          int num1;
          int num2;
          if (itemRepair.Point > 0 && itemRepair.Cash == 0)
          {
            int num3 = itemRepair.Quantity - (int) Item._count;
            int num4 = itemRepair.Point * num3;
            num1 = player._gp - num4;
            num2 = player._money;
            if (num1 < num4)
              this.Error = 2147483920U;
          }
          else if (itemRepair.Cash > 0 && itemRepair.Point == 0)
          {
            int num3 = itemRepair.Quantity - (int) Item._count;
            int num4 = itemRepair.Cash * num3;
            num1 = player._gp;
            num2 = player._money - num4;
            if (num2 < num4)
              this.Error = 2147483920U;
          }
          else
          {
            num1 = player._gp;
            num2 = player._money;
          }
          player._gp = num1;
          player._money = num2;
          Item._count = (long) itemRepair.Quantity;
          ComDiv.updateDB("accounts", "gp", (object) player._gp, "player_id", (object) player.player_id);
          ComDiv.updateDB("accounts", "money", (object) player._money, "player_id", (object) player.player_id);
          ComDiv.updateDB("player_items", "count", (object) Item._count, "object_id", (object) this.ObjectId);
          this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(3, player, Item));
          this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_REPAIR_ACK(this.Error, Item, player));
        }
        else
          this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_REPAIR_ACK(2147483920U, (ItemsModel) null, (PointBlank.Game.Data.Model.Account) null));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_SHOP_REPAIR_REQ: " + ex.ToString());
        this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_REPAIR_ACK(2147483648U, (ItemsModel) null, (PointBlank.Game.Data.Model.Account) null));
      }
    }
  }
}
