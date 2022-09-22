using PointBlank.Core.Models.Account.Players;

namespace PointBlank.Core.Models.Shop
{
  public class GoodItem
  {
    public ItemsModel _item = new ItemsModel() { _equip = 1 };
    public int price_gold;
    public int price_cash;
    public int auth_type;
    public int buy_type2;
    public int buy_type3;
    public int id;
    public int tag;
    public int title;
    public int visibility;
  }
}
