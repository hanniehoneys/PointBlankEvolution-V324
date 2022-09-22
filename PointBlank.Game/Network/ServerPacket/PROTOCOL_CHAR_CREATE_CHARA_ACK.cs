using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CHAR_CREATE_CHARA_ACK : SendPacket
  {
    private Character Character;
    private PointBlank.Game.Data.Model.Account Player;
    private uint Error;
    private int Type;

    public PROTOCOL_CHAR_CREATE_CHARA_ACK(
      uint Error,
      int Type,
      Character Character,
      PointBlank.Game.Data.Model.Account Player)
    {
      this.Character = Character;
      this.Player = Player;
      this.Error = Error;
      this.Type = Type;
    }

    public override void write()
    {
      this.writeH((short) 6146);
      this.writeH((short) 0);
      this.writeD(this.Error);
      if (this.Error == 0U)
      {
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
        this.writeD(this.Character.Id);
        this.writeD((int) this.Player._inventory.getItem(this.Character.Id)._objId);
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
        this.writeD((int) this.Character.ObjId);
        this.writeD(this.Character.CreateDate);
        this.writeD(0);
        this.writeD(0);
        this.writeUnicode(this.Character.Name, 66);
        this.writeD(this.Player._money);
        this.writeD(this.Player._gp);
        this.writeC((byte) this.Type);
        if (this.Character.Slot == 0 || this.Character.Slot == 1)
          this.writeC((byte) 20);
        else
          this.writeC((byte) 30);
        this.writeC((byte) this.Character.Slot);
        this.writeC((byte) 1);
      }
      else
        this.writeD(this.Error);
    }
  }
}
