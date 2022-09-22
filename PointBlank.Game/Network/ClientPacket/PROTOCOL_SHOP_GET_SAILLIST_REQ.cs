using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_SHOP_GET_SAILLIST_REQ : ReceivePacket
  {
    private string md5;

    public PROTOCOL_SHOP_GET_SAILLIST_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.md5 = this.readS(32);
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null)
          return;
        if (!player.LoadedShop)
        {
          player.LoadedShop = true;
          IEnumerable<IEnumerable<ShopData>> shopDatas1 = ShopManager.ShopDataItems.Split<ShopData>(808);
          IEnumerable<IEnumerable<ShopData>> shopDatas2 = ShopManager.ShopDataGoods.Split<ShopData>(153);
          IEnumerable<IEnumerable<ShopData>> shopDatas3 = ShopManager.ShopDataItemRepairs.Split<ShopData>(100);
          IEnumerable<IEnumerable<ShopData>> shopDatas4 = ShopManager.ShopDataMt1.Split<ShopData>(555);
          IEnumerable<IEnumerable<ShopData>> shopDatas5 = ShopManager.ShopDataMt2.Split<ShopData>(555);
          foreach (IEnumerable<ShopData> source in shopDatas1)
          {
            List<ShopData> list = source.ToList<ShopData>();
            for (int index = 0; index < list.Count; ++index)
              this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEMLIST_ACK(list[index], ShopManager.TotalItems));
          }
          foreach (IEnumerable<ShopData> source in shopDatas2)
          {
            List<ShopData> list = source.ToList<ShopData>();
            for (int index = 0; index < list.Count; ++index)
              this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GOODSLIST_ACK(list[index], ShopManager.TotalGoods));
          }
          foreach (IEnumerable<ShopData> source in shopDatas3)
          {
            List<ShopData> list = source.ToList<ShopData>();
            for (int index = 0; index < list.Count; ++index)
              this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_REPAIRLIST_ACK(list[index], ShopManager.TotalRepairs));
          }
          this._client.SendPacket((SendPacket) new PROTOCOL_BATTLEBOX_GET_LIST_ACK());
          if (player.pc_cafe == 0)
          {
            foreach (IEnumerable<ShopData> source in shopDatas4)
            {
              List<ShopData> list = source.ToList<ShopData>();
              for (int index = 0; index < list.Count; ++index)
                this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK(list[index], ShopManager.TotalMatching1));
            }
          }
          else
          {
            foreach (IEnumerable<ShopData> source in shopDatas5)
            {
              List<ShopData> list = source.ToList<ShopData>();
              for (int index = 0; index < list.Count; ++index)
                this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK(list[index], ShopManager.TotalMatching2));
            }
          }
        }
        if (ShopManager.ReadFile(Environment.CurrentDirectory + "/Data/Shop/Shop.dat") == this.md5)
          this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_GET_SAILLIST_ACK(false));
        else
          this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_GET_SAILLIST_ACK(true));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_SHOP_GET_SAILLIST_REQ: " + ex.ToString());
      }
    }
  }
}
