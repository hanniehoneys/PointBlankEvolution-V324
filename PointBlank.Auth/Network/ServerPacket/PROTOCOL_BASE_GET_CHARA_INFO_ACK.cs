using PointBlank.Core;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using System;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_GET_CHARA_INFO_ACK : SendPacket
  {
    private PointBlank.Auth.Data.Model.Account Player;

    public PROTOCOL_BASE_GET_CHARA_INFO_ACK(PointBlank.Auth.Data.Model.Account Player)
    {
      this.Player = Player;
    }

    public override void write()
    {
      try
      {
        this.writeH((short) 660);
        this.writeH((short) 0);
        this.writeC((byte) this.Player.Characters.Count);
        foreach (Character character in this.Player.Characters)
        {
          if (character != null)
          {
            this.writeC((byte) character.Slot);
            if (character.Slot == 0 || character.Slot == 1)
              this.writeC((byte) 20);
            else
              this.writeC((byte) 30);
            this.writeD((int) character.ObjId);
            this.writeD(character.CreateDate);
            this.writeD(character.PlayTime);
            this.writeD(character.PlayTime);
            this.writeUnicode(character.Name, 66);
            this.writeD(this.Player._equip._primary);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip._primary)._objId);
            this.writeD(this.Player._equip._secondary);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip._secondary)._objId);
            this.writeD(this.Player._equip._melee);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip._melee)._objId);
            this.writeD(this.Player._equip._grenade);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip._grenade)._objId);
            this.writeD(this.Player._equip._special);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip._special)._objId);
            this.writeD(character.Id);
            this.writeD((int) this.Player._inventory.getItem(character.Id)._objId);
            this.writeD(this.Player._equip.face);
            if (this.Player._inventory.getItem(this.Player._equip.face) == null)
              this.writeD(0);
            else
              this.writeD((int) this.Player._inventory.getItem(this.Player._equip.face)._objId);
            this.writeD(this.Player._equip._helmet);
            if (this.Player._inventory.getItem(this.Player._equip._helmet) == null)
              this.writeD(0);
            else
              this.writeD((int) this.Player._inventory.getItem(this.Player._equip._helmet)._objId);
            this.writeD(this.Player._equip.jacket);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip.jacket)._objId);
            this.writeD(this.Player._equip.poket);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip.poket)._objId);
            this.writeD(this.Player._equip.glove);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip.glove)._objId);
            this.writeD(this.Player._equip.belt);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip.belt)._objId);
            this.writeD(this.Player._equip.holster);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip.holster)._objId);
            this.writeD(this.Player._equip.skin);
            this.writeD((int) this.Player._inventory.getItem(this.Player._equip.skin)._objId);
            this.writeD(this.Player._equip._beret);
            if (this.Player._inventory.getItem(this.Player._equip._beret) == null)
              this.writeD(0);
            else
              this.writeD((int) this.Player._inventory.getItem(this.Player._equip._beret)._objId);
            this.writeC((byte) 0);
            this.writeC(byte.MaxValue);
            this.writeC(byte.MaxValue);
            this.writeC(byte.MaxValue);
            this.writeC((byte) 0);
            this.writeC((byte) 0);
            this.writeC((byte) 0);
          }
        }
        this.writeC((byte) 0);
      }
      catch (Exception ex)
      {
        Logger.error("PROTOCOL_BASE_GET_CHARA_INFO_ACK: " + ex.ToString());
      }
    }
  }
}
