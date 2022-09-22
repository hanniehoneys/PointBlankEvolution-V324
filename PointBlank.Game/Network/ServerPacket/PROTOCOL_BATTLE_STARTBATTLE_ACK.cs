using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_STARTBATTLE_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Room room;
    private PointBlank.Core.Models.Room.Slot slot;
    private int isBattle;
    private int type;
    private List<int> dinos;

    public PROTOCOL_BATTLE_STARTBATTLE_ACK(
      PointBlank.Core.Models.Room.Slot slot,
      Account pR,
      List<int> dinos,
      bool isBotMode,
      bool type)
    {
      this.slot = slot;
      this.room = pR._room;
      this.type = type ? 0 : 1;
      this.dinos = dinos;
      if (this.room == null)
        return;
      this.isBattle = 1;
      if (isBotMode || this.room.room_type == RoomType.Tutorial)
        return;
      AllUtils.CompleteMission(this.room, pR, slot, type ? MissionType.STAGE_ENTER : MissionType.STAGE_INTERCEPT, 0);
    }

    public PROTOCOL_BATTLE_STARTBATTLE_ACK()
    {
    }

    public override void write()
    {
      this.writeH((short) 4108);
      this.writeH((short) 0);
      this.writeD(0);
      this.writeC((byte) 26);
      if (this.isBattle != 1)
        return;
      if (this.room.room_type == RoomType.Boss || this.room.room_type == RoomType.CrossCounter)
      {
        int num1 = this.dinos.Count == 1 || this.room.room_type == RoomType.CrossCounter ? (int) byte.MaxValue : this.room.TRex;
        this.writeC((byte) num1);
        this.writeC((byte) 10);
        for (int index = 0; index < this.dinos.Count; ++index)
        {
          int dino = this.dinos[index];
          if (dino != this.room.TRex && this.room.room_type == RoomType.Boss || this.room.room_type == RoomType.CrossCounter)
            this.writeC((byte) dino);
        }
        int num2 = 8 - this.dinos.Count - (num1 == (int) byte.MaxValue ? 1 : 0);
        for (int index = 0; index < num2; ++index)
          this.writeC(byte.MaxValue);
        this.writeC(byte.MaxValue);
      }
      else
        this.writeB(new byte[10]);
      this.writeC((byte) this.room.rounds);
      if (this.room.room_type == RoomType.Bomb || this.room.room_type == RoomType.Annihilation || (this.room.room_type == RoomType.Convoy || this.room.room_type == RoomType.Destroy) || (this.room.room_type == RoomType.Defense || this.room.room_type == RoomType.Boss || (this.room.room_type == RoomType.CrossCounter || this.room.room_type == RoomType.FreeForAll)))
        this.writeH(AllUtils.getSlotsFlag(this.room, true, false));
      else
        this.writeC((byte) 1);
      this.writeC((byte) 2);
      if (this.room.room_type == RoomType.Bomb || this.room.room_type == RoomType.Annihilation || (this.room.room_type == RoomType.Convoy || this.room.room_type == RoomType.Destroy) || (this.room.room_type == RoomType.Defense || this.room.room_type == RoomType.FreeForAll))
      {
        this.writeH((ushort) this.room.red_rounds);
        this.writeH((ushort) this.room.blue_rounds);
      }
      else if (this.room.room_type == RoomType.Boss || this.room.room_type == RoomType.CrossCounter)
      {
        this.writeH(this.room.room_type == RoomType.CrossCounter ? (ushort) this.room._redKills : (ushort) this.room.red_dino);
        this.writeH(this.room.room_type == RoomType.CrossCounter ? (ushort) this.room._blueKills : (ushort) this.room.blue_dino);
      }
      else
        this.writeC((byte) 0);
      this.writeH(AllUtils.getSlotsFlag(this.room, false, false));
      this.writeC((byte) this.type);
      this.writeC((byte) this.slot._id);
    }
  }
}
