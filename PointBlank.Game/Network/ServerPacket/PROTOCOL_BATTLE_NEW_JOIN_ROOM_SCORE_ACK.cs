using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_NEW_JOIN_ROOM_SCORE_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Room room;

    public PROTOCOL_BATTLE_NEW_JOIN_ROOM_SCORE_ACK(PointBlank.Game.Data.Model.Room room)
    {
      this.room = room;
    }

    public override void write()
    {
      int inBattleTime = this.room.getInBattleTime();
      int num = !this.room.isBotMode() ? (this.room.room_type != RoomType.DeathMatch || this.room.isBotMode() ? (this.room.room_type == RoomType.Boss || this.room.room_type == RoomType.CrossCounter ? 4 : (this.room.room_type != RoomType.FreeForAll ? 2 : 5)) : 1) : 3;
      this.writeH((short) 4239);
      this.writeD(num);
      this.writeD(this.room.getTimeByMask() * 60 - inBattleTime);
      if (this.room.room_type == RoomType.Boss)
      {
        this.writeD(this.room.red_dino);
        this.writeD(this.room.blue_dino);
      }
      else if (this.room.room_type == RoomType.DeathMatch)
      {
        this.writeD(this.room._redKills);
        this.writeD(this.room._blueKills);
      }
      else if (this.room.room_type == RoomType.FreeForAll)
      {
        this.writeD(this.GetSlotKill());
        this.writeD(0);
      }
      else if (this.room.isBotMode())
      {
        this.writeD((int) this.room.IngameAiLevel);
        this.writeD(0);
      }
      else
      {
        this.writeD(this.room.red_rounds);
        this.writeD(this.room.blue_rounds);
      }
    }

    public int GetSlotKill()
    {
      int[] numArray = new int[16];
      for (int slotIdx = 0; slotIdx < numArray.Length; ++slotIdx)
      {
        Slot slot = this.room.getSlot(slotIdx);
        numArray[slotIdx] = slot.allKills;
      }
      int index1 = 0;
      for (int index2 = 0; index2 < numArray.Length; ++index2)
      {
        if (numArray[index2] > numArray[index1])
          index1 = index2;
      }
      return index1;
    }
  }
}
