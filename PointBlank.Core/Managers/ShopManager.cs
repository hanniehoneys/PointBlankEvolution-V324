using Npgsql;
using PointBlank.Core.Models.Shop;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace PointBlank.Core.Managers
{
  public static class ShopManager
  {
    public static List<ItemRepair> ItemRepairs = new List<ItemRepair>();
    public static List<GoodItem> ShopAllList = new List<GoodItem>();
    public static List<GoodItem> ShopBuyableList = new List<GoodItem>();
    public static SortedList<int, GoodItem> ShopUniqueList = new SortedList<int, GoodItem>();
    public static List<ShopData> ShopDataMt1 = new List<ShopData>();
    public static List<ShopData> ShopDataMt2 = new List<ShopData>();
    public static List<ShopData> ShopDataGoods = new List<ShopData>();
    public static List<ShopData> ShopDataItems = new List<ShopData>();
    public static List<ShopData> ShopDataItemRepairs = new List<ShopData>();
    public static int TotalGoods;
    public static int TotalItems;
    public static int TotalMatching1;
    public static int TotalMatching2;
    public static int set4p;
    public static int TotalRepairs;

    public static IEnumerable<IEnumerable<T>> Split<T>(
      this IEnumerable<T> list,
      int limit)
    {
      return list.Select((item, inx) => new { item, inx }).GroupBy(x => x.inx / limit).Select(g => g.Select(x => x.item));
    }

    public static void Load(int type)
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          npgsqlConnection.Open();
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          command.CommandText = "SELECT * FROM shop";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            GoodItem goodItem = new GoodItem() { id = npgsqlDataReader.GetInt32(0), price_gold = npgsqlDataReader.GetInt32(3), price_cash = npgsqlDataReader.GetInt32(4), auth_type = npgsqlDataReader.GetInt32(6), buy_type2 = npgsqlDataReader.GetInt32(7), buy_type3 = npgsqlDataReader.GetInt32(8), tag = npgsqlDataReader.GetInt32(9), title = npgsqlDataReader.GetInt32(10), visibility = npgsqlDataReader.GetInt32(11) };
            goodItem._item.SetItemId(npgsqlDataReader.GetInt32(1));
            goodItem._item._name = npgsqlDataReader.GetString(2);
            goodItem._item._count = (long) npgsqlDataReader.GetInt32(5);
            int idStatics = ComDiv.getIdStatics(goodItem._item._id, 1);
            int num;
            switch (type)
            {
              case 1:
                num = 1;
                break;
              case 2:
                num = idStatics == 16 ? 1 : 0;
                break;
              default:
                num = 0;
                break;
            }
            if (num != 0)
            {
              ShopManager.ShopAllList.Add(goodItem);
              if (goodItem.visibility != 2 && goodItem.visibility != 4)
                ShopManager.ShopBuyableList.Add(goodItem);
              if (!ShopManager.ShopUniqueList.ContainsKey(goodItem._item._id) && goodItem.auth_type > 0)
              {
                ShopManager.ShopUniqueList.Add(goodItem._item._id, goodItem);
                if (goodItem.visibility == 4)
                  ++ShopManager.set4p;
              }
            }
          }
          if (type == 1)
          {
            ShopManager.LoadItemRepair();
            ShopManager.LoadDataMatching1Goods(0);
            ShopManager.LoadDataMatching2(1);
            ShopManager.LoadDataItems();
            ShopManager.LoadDataItemRepairs();
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    public static void LoadItemRepair()
    {
      try
      {
        using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
        {
          npgsqlConnection.Open();
          NpgsqlCommand command = npgsqlConnection.CreateCommand();
          command.CommandText = "SELECT * FROM shop_item_repair";
          command.CommandType = CommandType.Text;
          NpgsqlDataReader npgsqlDataReader = command.ExecuteReader();
          while (npgsqlDataReader.Read())
          {
            ItemRepair itemRepair = new ItemRepair() { ItemId = npgsqlDataReader.GetInt32(0), Point = npgsqlDataReader.GetInt32(1), Cash = npgsqlDataReader.GetInt32(2), Quantity = npgsqlDataReader.GetInt32(3), Enable = npgsqlDataReader.GetBoolean(4) };
            if (itemRepair.Enable)
              ShopManager.ItemRepairs.Add(itemRepair);
          }
          command.Dispose();
          npgsqlDataReader.Close();
          npgsqlConnection.Dispose();
          npgsqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    public static void Reset()
    {
      ShopManager.set4p = 0;
      ShopManager.ShopAllList.Clear();
      ShopManager.ShopBuyableList.Clear();
      ShopManager.ShopUniqueList.Clear();
      ShopManager.ShopDataMt1.Clear();
      ShopManager.ShopDataMt2.Clear();
      ShopManager.ShopDataGoods.Clear();
      ShopManager.ShopDataItems.Clear();
      ShopManager.ShopDataItemRepairs.Clear();
      ShopManager.ItemRepairs.Clear();
      ShopManager.TotalGoods = 0;
      ShopManager.TotalItems = 0;
      ShopManager.TotalMatching1 = 0;
      ShopManager.TotalMatching2 = 0;
    }

    private static void LoadDataMatching1Goods(int cafe)
    {
      List<GoodItem> list1 = new List<GoodItem>();
      List<GoodItem> list2 = new List<GoodItem>();
      lock (ShopManager.ShopAllList)
      {
        for (int index = 0; index < ShopManager.ShopAllList.Count; ++index)
        {
          GoodItem shopAll = ShopManager.ShopAllList[index];
          if (shopAll._item._count != 0L)
          {
            if ((shopAll.tag != 4 || cafe != 0) && (shopAll.tag == 4 && cafe > 0 || shopAll.visibility != 2))
              list1.Add(shopAll);
            if (shopAll.visibility < 2 || shopAll.visibility == 4)
              list2.Add(shopAll);
          }
        }
      }
      ShopManager.TotalMatching1 = list1.Count;
      ShopManager.TotalGoods = list2.Count;
      int num1 = (int) Math.Ceiling((double) list1.Count / 555.0);
      int count = 0;
      for (int page = 0; page < num1; ++page)
      {
        byte[] matchingData = ShopManager.getMatchingData(555, page, ref count, list1);
        ShopData shopData = new ShopData() { Buffer = matchingData, ItemsCount = count, Offset = page * 555 };
        ShopManager.ShopDataMt1.Add(shopData);
      }
      int num2 = (int) Math.Ceiling((double) list2.Count / 153.0);
      for (int page = 0; page < num2; ++page)
      {
        byte[] goodsData = ShopManager.getGoodsData(153, page, ref count, list2);
        ShopData shopData = new ShopData() { Buffer = goodsData, ItemsCount = count, Offset = page * 153 };
        ShopManager.ShopDataGoods.Add(shopData);
      }
    }

    private static void LoadDataMatching2(int cafe)
    {
      List<GoodItem> list = new List<GoodItem>();
      lock (ShopManager.ShopAllList)
      {
        for (int index = 0; index < ShopManager.ShopAllList.Count; ++index)
        {
          GoodItem shopAll = ShopManager.ShopAllList[index];
          if (shopAll._item._count != 0L && ((shopAll.tag != 4 || cafe != 0) && (shopAll.tag == 4 && cafe > 0 || shopAll.visibility != 2)))
            list.Add(shopAll);
        }
      }
      ShopManager.TotalMatching2 = list.Count;
      int num = (int) Math.Ceiling((double) list.Count / 555.0);
      int count = 0;
      for (int page = 0; page < num; ++page)
      {
        byte[] matchingData = ShopManager.getMatchingData(555, page, ref count, list);
        ShopData shopData = new ShopData() { Buffer = matchingData, ItemsCount = count, Offset = page * 555 };
        ShopManager.ShopDataMt2.Add(shopData);
      }
    }

    private static void LoadDataItems()
    {
      List<GoodItem> list = new List<GoodItem>();
      lock (ShopManager.ShopUniqueList)
      {
        for (int index = 0; index < ShopManager.ShopUniqueList.Values.Count; ++index)
        {
          GoodItem goodItem = ShopManager.ShopUniqueList.Values[index];
          if (goodItem.visibility != 1 && goodItem.visibility != 3)
            list.Add(goodItem);
        }
      }
      ShopManager.TotalItems = list.Count;
      int num = (int) Math.Ceiling((double) list.Count / 808.0);
      int count = 0;
      for (int page = 0; page < num; ++page)
      {
        byte[] itemsData = ShopManager.getItemsData(808, page, ref count, list);
        ShopData shopData = new ShopData() { Buffer = itemsData, ItemsCount = count, Offset = page * 808 };
        ShopManager.ShopDataItems.Add(shopData);
      }
    }

    private static void LoadDataItemRepairs()
    {
      List<ItemRepair> list = new List<ItemRepair>();
      lock (ShopManager.ItemRepairs)
      {
        for (int index = 0; index < ShopManager.ItemRepairs.Count; ++index)
        {
          ItemRepair itemRepair = ShopManager.ItemRepairs[index];
          list.Add(itemRepair);
        }
      }
      ShopManager.TotalRepairs = list.Count;
      int num = (int) Math.Ceiling((double) list.Count / 100.0);
      int count = 0;
      for (int page = 0; page < num; ++page)
      {
        byte[] repairsData = ShopManager.getRepairsData(100, page, ref count, list);
        ShopData shopData = new ShopData() { Buffer = repairsData, ItemsCount = count, Offset = page * 100 };
        ShopManager.ShopDataItemRepairs.Add(shopData);
      }
    }

    private static byte[] getItemsData(int maximum, int page, ref int count, List<GoodItem> list)
    {
      count = 0;
      using (SendGPacket p = new SendGPacket())
      {
        for (int index = page * maximum; index < list.Count; ++index)
        {
          ShopManager.WriteItemsData(list[index], p);
          if (++count == maximum)
            break;
        }
        return p.mstream.ToArray();
      }
    }

    private static byte[] getRepairsData(
      int maximum,
      int page,
      ref int count,
      List<ItemRepair> list)
    {
      count = 0;
      using (SendGPacket p = new SendGPacket())
      {
        for (int index = page * maximum; index < list.Count; ++index)
        {
          ShopManager.WriteRepairsData(list[index], p);
          if (++count == maximum)
            break;
        }
        return p.mstream.ToArray();
      }
    }

    private static byte[] getGoodsData(int maximum, int page, ref int count, List<GoodItem> list)
    {
      count = 0;
      using (SendGPacket p = new SendGPacket())
      {
        for (int index = page * maximum; index < list.Count; ++index)
        {
          ShopManager.WriteGoodsData(list[index], p);
          if (++count == maximum)
            break;
        }
        return p.mstream.ToArray();
      }
    }

    private static byte[] getMatchingData(
      int maximum,
      int page,
      ref int count,
      List<GoodItem> list)
    {
      count = 0;
      using (SendGPacket p = new SendGPacket())
      {
        for (int index = page * maximum; index < list.Count; ++index)
        {
          ShopManager.WriteMatchData(list[index], p);
          if (++count == maximum)
            break;
        }
        return p.mstream.ToArray();
      }
    }

    private static void WriteItemsData(GoodItem good, SendGPacket p)
    {
      p.writeD(good._item._id);
      p.writeC((byte) good.auth_type);
      p.writeC((byte) good.buy_type2);
      p.writeC((byte) good.buy_type3);
      p.writeC((byte) good.title);
      p.writeC(good.title != 0 ? (byte) 2 : (byte) 0);
      p.writeH((short) 0);
    }

    private static void WriteRepairsData(ItemRepair repair, SendGPacket p)
    {
      p.writeD(repair.ItemId);
      p.writeD(repair.Point);
      p.writeD(repair.Cash);
      p.writeD(repair.Quantity);
    }

    private static void WriteGoodsData(GoodItem good, SendGPacket p)
    {
      p.writeD(good.id);
      p.writeC((byte) 1);
      p.writeC(good.visibility == 4 ? (byte) 4 : (byte) 1);
      p.writeD(good.price_gold);
      p.writeD(good.price_cash);
      p.writeD(0);
      p.writeC((byte) good.tag);
      p.writeB(new byte[39]);
    }

    private static void WriteMatchData(GoodItem good, SendGPacket p)
    {
      p.writeD(good.id);
      p.writeD(good._item._id);
      p.writeD((int) good._item._count);
      p.writeD(0);
    }

    public static bool IsBlocked(string txt, List<int> items)
    {
      lock (ShopManager.ShopUniqueList)
      {
        for (int index = 0; index < ShopManager.ShopUniqueList.Values.Count; ++index)
        {
          GoodItem goodItem = ShopManager.ShopUniqueList.Values[index];
          if (!items.Contains(goodItem._item._id) && goodItem._item._name.Contains(txt))
            items.Add(goodItem._item._id);
        }
      }
      return false;
    }

    public static GoodItem getGood(int goodId)
    {
      if (goodId == 0)
        return (GoodItem) null;
      lock (ShopManager.ShopAllList)
      {
        for (int index = 0; index < ShopManager.ShopAllList.Count; ++index)
        {
          GoodItem shopAll = ShopManager.ShopAllList[index];
          if (shopAll.id == goodId)
            return shopAll;
        }
      }
      return (GoodItem) null;
    }

    public static GoodItem getItemId(int ItemId)
    {
      if (ItemId == 0)
        return (GoodItem) null;
      lock (ShopManager.ShopAllList)
      {
        for (int index = 0; index < ShopManager.ShopAllList.Count; ++index)
        {
          GoodItem shopAll = ShopManager.ShopAllList[index];
          if (shopAll._item._id == ItemId)
            return shopAll;
        }
      }
      return (GoodItem) null;
    }

    public static List<GoodItem> getGoods(
      List<CartGoods> ShopCart,
      out int GoldPrice,
      out int CashPrice)
    {
      GoldPrice = 0;
      CashPrice = 0;
      List<GoodItem> goodItemList = new List<GoodItem>();
      if (ShopCart.Count == 0)
        return goodItemList;
      lock (ShopManager.ShopBuyableList)
      {
        for (int index1 = 0; index1 < ShopManager.ShopBuyableList.Count; ++index1)
        {
          GoodItem shopBuyable = ShopManager.ShopBuyableList[index1];
          for (int index2 = 0; index2 < ShopCart.Count; ++index2)
          {
            CartGoods cartGoods = ShopCart[index2];
            if (cartGoods.GoodId == shopBuyable.id)
            {
              goodItemList.Add(shopBuyable);
              if (cartGoods.BuyType == 1)
                GoldPrice += shopBuyable.price_gold;
              else if (cartGoods.BuyType == 2)
                CashPrice += shopBuyable.price_cash;
            }
          }
        }
      }
      return goodItemList;
    }

    public static string ReadFile(string Path)
    {
      string str = "";
      using (MD5 md5 = MD5.Create())
      {
        using (FileStream fileStream = File.OpenRead(Path))
        {
          str = BitConverter.ToString(md5.ComputeHash((Stream) fileStream)).Replace("-", string.Empty);
          fileStream.Close();
        }
      }
      return str;
    }
  }
}
