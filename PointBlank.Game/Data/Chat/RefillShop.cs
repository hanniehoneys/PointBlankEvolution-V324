using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Data.Chat
{
  public static class RefillShop
  {
    public static string SimpleRefill(Account player)
    {
      ShopManager.Reset();
      ShopManager.Load(1);
      Logger.warning(Translation.GetLabel("RefillShopWarn", (object) player.player_name));
      return Translation.GetLabel("RefillShopMsg");
    }

    public static string InstantRefill(Account player)
    {
      ShopManager.Reset();
      ShopManager.Load(1);
      for (int index = 0; index < ShopManager.ShopDataItems.Count; ++index)
      {
        ShopData shopDataItem = ShopManager.ShopDataItems[index];
        player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEMLIST_ACK(shopDataItem, ShopManager.TotalItems));
      }
      for (int index = 0; index < ShopManager.ShopDataGoods.Count; ++index)
      {
        ShopData shopDataGood = ShopManager.ShopDataGoods[index];
        player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GOODSLIST_ACK(shopDataGood, ShopManager.TotalGoods));
      }
      for (int index = 0; index < ShopManager.ShopDataItemRepairs.Count; ++index)
      {
        ShopData shopDataItemRepair = ShopManager.ShopDataItemRepairs[index];
        player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_REPAIRLIST_ACK(shopDataItemRepair, ShopManager.TotalRepairs));
      }
      if (player.pc_cafe == 0)
      {
        for (int index = 0; index < ShopManager.ShopDataMt1.Count; ++index)
        {
          ShopData data = ShopManager.ShopDataMt1[index];
          player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK(data, ShopManager.TotalMatching1));
        }
      }
      else
      {
        for (int index = 0; index < ShopManager.ShopDataMt2.Count; ++index)
        {
          ShopData data = ShopManager.ShopDataMt2[index];
          player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK(data, ShopManager.TotalMatching2));
        }
      }
      player.SendPacket((SendPacket) new PROTOCOL_SHOP_GET_SAILLIST_ACK(true));
      Logger.warning(Translation.GetLabel("RefillShopWarn", (object) player.player_name));
      return Translation.GetLabel("RefillShopMsg");
    }
  }
}
