using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_GET_USER_ITEM_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Account p;

    public PROTOCOL_ROOM_GET_USER_ITEM_ACK(PointBlank.Game.Data.Model.Account p)
    {
      this.p = p;
    }

    public override void write()
    {
      List<ItemsModel> itemsByType = this.p._inventory.getItemsByType(4);
      this.writeH((short) 3900);
      this.writeH((short) 0);
      this.writeH((short) itemsByType.Count);
      for (int index = 0; index < itemsByType.Count; ++index)
        this.writeD(itemsByType[index]._id);
      this.writeD(this.p._equip._dino);
      this.writeD((int) this.p._inventory.getItem(this.p._equip._dino)._objId);
      this.writeD(this.p._equip._primary);
      this.writeD((int) this.p._inventory.getItem(this.p._equip._primary)._objId);
      this.writeD(this.p._equip._secondary);
      this.writeD((int) this.p._inventory.getItem(this.p._equip._secondary)._objId);
      this.writeD(this.p._equip._melee);
      this.writeD((int) this.p._inventory.getItem(this.p._equip._melee)._objId);
      this.writeD(this.p._equip._grenade);
      this.writeD((int) this.p._inventory.getItem(this.p._equip._grenade)._objId);
      this.writeD(this.p._equip._special);
      this.writeD((int) this.p._inventory.getItem(this.p._equip._special)._objId);
      if (this.p._slotId % 2 == 0)
      {
        this.writeD(this.p._equip._red);
        this.writeD((int) this.p._inventory.getItem(this.p._equip._red)._objId);
      }
      else
      {
        this.writeD(this.p._equip._blue);
        this.writeD((int) this.p._inventory.getItem(this.p._equip._blue)._objId);
      }
      this.writeD(this.p._equip.face);
      if (this.p._equip.face == 0)
        this.writeD(0);
      else
        this.writeD((uint) this.p._inventory.getItem(this.p._equip.face)._objId);
      this.writeD(this.p._equip._helmet);
      if (this.p._equip._helmet == 0)
        this.writeD(0);
      else
        this.writeD((uint) this.p._inventory.getItem(this.p._equip._helmet)._objId);
      this.writeD(this.p._equip.jacket);
      this.writeD((uint) this.p._inventory.getItem(this.p._equip.jacket)._objId);
      this.writeD(this.p._equip.poket);
      this.writeD((uint) this.p._inventory.getItem(this.p._equip.poket)._objId);
      this.writeD(this.p._equip.glove);
      this.writeD((uint) this.p._inventory.getItem(this.p._equip.glove)._objId);
      this.writeD(this.p._equip.belt);
      this.writeD((uint) this.p._inventory.getItem(this.p._equip.belt)._objId);
      this.writeD(this.p._equip.holster);
      this.writeD((uint) this.p._inventory.getItem(this.p._equip.holster)._objId);
      this.writeD(this.p._equip.skin);
      this.writeD((uint) this.p._inventory.getItem(this.p._equip.skin)._objId);
      this.writeD(this.p._equip._beret);
      if (this.p._equip._beret == 0)
        this.writeD(0);
      else
        this.writeD((uint) this.p._inventory.getItem(this.p._equip._beret)._objId);
    }
  }
}
