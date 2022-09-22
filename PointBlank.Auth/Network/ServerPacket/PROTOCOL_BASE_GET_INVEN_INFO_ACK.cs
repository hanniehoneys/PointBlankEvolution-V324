using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_GET_INVEN_INFO_ACK : SendPacket
  {
    private List<ItemsModel> charas = new List<ItemsModel>();
    private List<ItemsModel> weapons = new List<ItemsModel>();
    private List<ItemsModel> cupons = new List<ItemsModel>();

    public PROTOCOL_BASE_GET_INVEN_INFO_ACK(List<ItemsModel> items)
    {
      this.InventoryLoad(items);
    }

    private void InventoryLoad(List<ItemsModel> items)
    {
      for (int index = 0; index < items.Count; ++index)
      {
        ItemsModel itemsModel = items[index];
        if (itemsModel._category == 1)
          this.weapons.Add(itemsModel);
        else if (itemsModel._category == 2)
          this.charas.Add(itemsModel);
        else if (itemsModel._category == 3)
          this.cupons.Add(itemsModel);
      }
    }

    public override void write()
    {
      int num = this.charas.Count + this.weapons.Count + this.cupons.Count;
      this.writeH((short) 527);
      this.writeH((short) 0);
      this.writeD(0);
      this.writeH((short) num);
      foreach (ItemsModel chara in this.charas)
      {
        this.writeD((uint) chara._objId);
        this.writeD(chara._id);
        this.writeC((byte) chara._equip);
        this.writeD((int) chara._count);
      }
      foreach (ItemsModel weapon in this.weapons)
      {
        this.writeD((uint) weapon._objId);
        this.writeD(weapon._id);
        this.writeC((byte) weapon._equip);
        this.writeD((int) weapon._count);
      }
      foreach (ItemsModel cupon in this.cupons)
      {
        this.writeD((uint) cupon._objId);
        this.writeD(cupon._id);
        this.writeC((byte) cupon._equip);
        this.writeD((int) cupon._count);
      }
    }
  }
}
