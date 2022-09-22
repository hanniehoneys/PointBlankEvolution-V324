using PointBlank.Core.Models.Account.Title;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_START_GAME_TRANS_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Room Room;
    private Slot slot;
    private PlayerTitles title;

    public PROTOCOL_BATTLE_START_GAME_TRANS_ACK(PointBlank.Game.Data.Model.Room Room, Slot slot, PlayerTitles title)
    {
      this.Room = Room;
      this.slot = slot;
      this.title = title;
    }

    public override void write()
    {
      if (this.slot._equip == null)
        return;
      this.writeH((short) 4104);
      this.writeH((short) 0);
      this.writeD((uint) this.slot._playerId);
      this.writeC((byte) 2);
      this.writeC((byte) this.slot._id);
      if (this.slot._id % 2 == 0)
        this.writeD(this.slot._equip._red);
      else
        this.writeD(this.slot._equip._blue);
      this.writeD(this.slot._equip._primary);
      this.writeD(this.slot._equip._secondary);
      this.writeD(this.slot._equip._melee);
      this.writeD(this.slot._equip._grenade);
      this.writeD(this.slot._equip._special);
      if (this.Room.room_type == RoomType.Boss || this.Room.room_type == RoomType.CrossCounter)
      {
        if (!this.Room.swapRound)
        {
          if (this.slot._id % 2 == 0)
            this.writeD(this.slot._equip._dino);
          else
            this.writeD(this.slot._equip._blue);
        }
        else if (this.slot._id % 2 == 0)
          this.writeD(this.slot._equip._blue);
        else
          this.writeD(this.slot._equip._dino);
      }
      else if (this.slot._id % 2 == 0)
        this.writeD(this.slot._equip._red);
      else
        this.writeD(this.slot._equip._blue);
      this.writeD(this.slot._equip.face);
      this.writeD(this.slot._equip._helmet);
      this.writeD(this.slot._equip.jacket);
      this.writeD(this.slot._equip.poket);
      this.writeD(this.slot._equip.glove);
      this.writeD(this.slot._equip.belt);
      this.writeD(this.slot._equip.holster);
      this.writeD(this.slot._equip.skin);
      this.writeD(this.slot._equip._beret);
      this.writeC((byte) 100);
      this.writeC((byte) 100);
      this.writeC((byte) 100);
      this.writeC((byte) 100);
      this.writeC((byte) 100);
      this.writeC((byte) this.title.Equiped1);
      this.writeC((byte) this.title.Equiped2);
      this.writeC((byte) this.title.Equiped3);
      this.writeC((byte) 0);
      this.writeC(byte.MaxValue);
      this.writeC(byte.MaxValue);
      this.writeC(byte.MaxValue);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
    }
  }
}
