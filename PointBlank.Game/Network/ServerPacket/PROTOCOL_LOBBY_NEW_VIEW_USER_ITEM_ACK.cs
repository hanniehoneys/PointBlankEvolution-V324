using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_LOBBY_NEW_VIEW_USER_ITEM_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Account ac;

    public PROTOCOL_LOBBY_NEW_VIEW_USER_ITEM_ACK(long player)
    {
      this.ac = AccountManager.getAccount(player, true);
    }

    public override void write()
    {
      List<ItemsModel> itemsByType = this.ac._inventory.getItemsByType(4);
      this.writeH((short) 3094);
      this.writeH((short) 0);
      this.writeH((short) itemsByType.Count);
      for (int index = 0; index < itemsByType.Count; ++index)
        this.writeD(itemsByType[index]._id);
      this.writeD(this.ac._equip._dino);
      this.writeC((byte) 15);
      this.writeD(this.ac._equip._primary);
      this.writeD(this.ac._equip._secondary);
      this.writeD(this.ac._equip._melee);
      this.writeD(this.ac._equip._grenade);
      this.writeD(this.ac._equip._special);
      this.writeD(this.ac._equip._red);
      this.writeD(this.ac._equip.face);
      this.writeD(this.ac._equip._helmet);
      this.writeD(this.ac._equip.jacket);
      this.writeD(this.ac._equip.poket);
      this.writeD(this.ac._equip.glove);
      this.writeD(this.ac._equip.belt);
      this.writeD(this.ac._equip.holster);
      this.writeD(this.ac._equip.skin);
      this.writeD(this.ac._equip._beret);
      this.writeC((byte) 2);
      this.writeD(this.ac._equip._red);
      this.writeD(this.ac._equip._blue);
    }
  }
}
