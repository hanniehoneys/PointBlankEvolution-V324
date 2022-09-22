using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SERVER_MESSAGE_CHANGE_INVENTORY_ACK : SendPacket
  {
    private PlayerEquipedItems Equip;
    private PointBlank.Game.Data.Model.Account Player;

    public PROTOCOL_SERVER_MESSAGE_CHANGE_INVENTORY_ACK(PointBlank.Game.Data.Model.Account Player)
    {
      this.Player = Player;
      PlayerManager.CheckEquipedItems(Player._equip, Player._inventory._items, false);
      this.Equip = Player._equip;
    }

    public override void write()
    {
      List<ItemsModel> itemsByType = this.Player._inventory.getItemsByType(4);
      this.writeH((short) 2570);
      this.writeH((short) 0);
      this.writeC((byte) itemsByType.Count);
      for (int index = 0; index < itemsByType.Count; ++index)
        this.writeD(itemsByType[index]._id);
      this.writeC((byte) itemsByType.Count);
      this.writeC((byte) 0);
      this.writeC((byte) this.Player.Characters.Count);
      for (int index = 0; index < this.Player.Characters.Count; ++index)
      {
        Character character = this.Player.Characters[index];
        this.writeC((byte) character.Slot);
        this.writeD(this.Equip._primary);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip._primary)._objId);
        this.writeD(this.Equip._secondary);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip._secondary)._objId);
        this.writeD(this.Equip._melee);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip._melee)._objId);
        this.writeD(this.Equip._grenade);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip._grenade)._objId);
        this.writeD(this.Equip._special);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip._special)._objId);
        this.writeD(character.Id);
        this.writeD((uint) this.Player._inventory.getItem(character.Id)._objId);
        this.writeD(this.Equip.face);
        if (this.Equip.face == 0)
          this.writeD(0);
        else
          this.writeD((uint) this.Player._inventory.getItem(this.Equip.face)._objId);
        this.writeD(this.Equip._helmet);
        if (this.Equip._helmet == 0)
          this.writeD(0);
        else
          this.writeD((uint) this.Player._inventory.getItem(this.Equip._helmet)._objId);
        this.writeD(this.Equip.jacket);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip.jacket)._objId);
        this.writeD(this.Equip.poket);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip.poket)._objId);
        this.writeD(this.Equip.glove);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip.glove)._objId);
        this.writeD(this.Equip.belt);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip.belt)._objId);
        this.writeD(this.Equip.holster);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip.holster)._objId);
        this.writeD(this.Equip.skin);
        this.writeD((uint) this.Player._inventory.getItem(this.Equip.skin)._objId);
        this.writeD(this.Equip._beret);
        if (this.Equip._beret == 0)
          this.writeD(0);
        else
          this.writeD((uint) this.Player._inventory.getItem(this.Equip._beret)._objId);
      }
      this.writeD(this.Equip._dino);
      this.writeD((uint) this.Player._inventory.getItem(this.Equip._dino)._objId);
    }
  }
}
