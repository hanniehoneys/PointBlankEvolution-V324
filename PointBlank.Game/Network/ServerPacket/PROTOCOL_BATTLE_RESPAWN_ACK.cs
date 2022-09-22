using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Utils;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_RESPAWN_ACK : SendPacket
  {
    private PointBlank.Core.Models.Room.Slot slot;
    private PointBlank.Game.Data.Model.Room room;

    public PROTOCOL_BATTLE_RESPAWN_ACK(PointBlank.Game.Data.Model.Room r, PointBlank.Core.Models.Room.Slot slot)
    {
      this.slot = slot;
      this.room = r;
    }

    public override void write()
    {
      bool flag = this.slot._id % 2 == 0;
      this.writeH((short) 4114);
      this.writeD(this.slot._id);
      this.writeD(this.room.spawnsCount++);
      this.writeD(++this.slot.spawnsCount);
      this.writeD(this.slot._equip._primary);
      this.writeD(this.slot._equip._secondary);
      this.writeD(this.slot._equip._melee);
      this.writeD(this.slot._equip._grenade);
      this.writeD(this.slot._equip._special);
      this.writeB(new byte[5]
      {
        (byte) 100,
        (byte) 100,
        (byte) 100,
        (byte) 100,
        (byte) 100
      });
      if (this.room.room_type == RoomType.Boss || this.room.room_type == RoomType.CrossCounter)
      {
        if (!this.room.swapRound)
        {
          if (flag)
            this.writeD(this.slot._equip._dino);
          else
            this.writeD(this.slot._equip._blue);
        }
        else if (flag)
          this.writeD(this.slot._equip._blue);
        else
          this.writeD(this.slot._equip._dino);
      }
      else if (flag)
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
      if (this.room.room_type != RoomType.Boss && this.room.room_type != RoomType.CrossCounter)
        return;
      List<int> dinossaurs = AllUtils.getDinossaurs(this.room, false, this.slot._id);
      int num1 = dinossaurs.Count == 1 || this.room.room_type == RoomType.CrossCounter ? (int) byte.MaxValue : this.room.TRex;
      this.writeC((byte) num1);
      this.writeC((byte) 10);
      for (int index = 0; index < dinossaurs.Count; ++index)
      {
        int num2 = dinossaurs[index];
        if (num2 != this.room.TRex && this.room.room_type == RoomType.Boss || this.room.room_type == RoomType.CrossCounter)
          this.writeC((byte) num2);
      }
      int num3 = 8 - dinossaurs.Count - (num1 == (int) byte.MaxValue ? 1 : 0);
      for (int index = 0; index < num3; ++index)
        this.writeC(byte.MaxValue);
      this.writeC(byte.MaxValue);
    }
  }
}
