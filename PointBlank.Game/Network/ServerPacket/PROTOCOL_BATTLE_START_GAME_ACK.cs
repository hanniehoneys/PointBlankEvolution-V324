using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_START_GAME_ACK : SendPacket
  {
    private int PlayersCount;
    private PointBlank.Game.Data.Model.Room room;
    private byte[] Data;

    public PROTOCOL_BATTLE_START_GAME_ACK(PointBlank.Game.Data.Model.Room r)
    {
      this.room = r;
      if (GameConfig.isTestMode && GameConfig.udpType == UdpState.RELAY)
      {
        this.room._slots[1]._playerId = 0L;
        this.room._slots[1].state = SlotState.EMPTY;
      }
      using (SendGPacket pk = new SendGPacket())
      {
        for (int index = 0; index < 16; ++index)
        {
          Slot slot = this.room._slots[index];
          if (slot.state >= SlotState.READY && slot._equip != null)
          {
            Account playerBySlot = this.room.getPlayerBySlot(slot);
            if (playerBySlot != null && playerBySlot._slotId == index)
            {
              this.WriteSlotInfo(slot, playerBySlot, pk);
              ++this.PlayersCount;
            }
          }
        }
        this.Data = pk.mstream.ToArray();
      }
    }

    private void WriteSlotInfo(Slot s, Account p, SendGPacket pk)
    {
      pk.writeC((byte) s._id);
      if (s._id % 2 == 0)
        pk.writeD(s._equip._red);
      else
        pk.writeD(s._equip._blue);
      pk.writeD(s._equip._primary);
      pk.writeD(s._equip._secondary);
      pk.writeD(s._equip._melee);
      pk.writeD(s._equip._grenade);
      pk.writeD(s._equip._special);
      if (this.room.room_type == RoomType.Boss || this.room.room_type == RoomType.CrossCounter)
      {
        if (!this.room.swapRound)
        {
          if (s._id % 2 == 0)
            pk.writeD(s._equip._dino);
          else
            pk.writeD(s._equip._blue);
        }
      }
      else if (s._id % 2 == 0)
        pk.writeD(s._equip._blue);
      else
        pk.writeD(s._equip._dino);
      pk.writeD(s._equip.face);
      pk.writeD(s._equip._helmet);
      pk.writeD(s._equip.jacket);
      pk.writeD(s._equip.poket);
      pk.writeD(s._equip.glove);
      pk.writeD(s._equip.belt);
      pk.writeD(s._equip.holster);
      pk.writeD(s._equip.skin);
      pk.writeD(s._equip._beret);
      pk.writeC((byte) 100);
      pk.writeC((byte) 100);
      pk.writeC((byte) 100);
      pk.writeC((byte) 100);
      pk.writeC((byte) 100);
      if (p != null)
      {
        pk.writeC((byte) p._titles.Equiped1);
        pk.writeC((byte) p._titles.Equiped2);
        pk.writeC((byte) p._titles.Equiped3);
      }
      else
        pk.writeB(new byte[3]);
      pk.writeC((byte) 0);
      pk.writeC(byte.MaxValue);
      pk.writeC(byte.MaxValue);
      pk.writeC(byte.MaxValue);
      pk.writeC((byte) 0);
      pk.writeC((byte) 0);
      pk.writeC((byte) 0);
    }

    public override void write()
    {
      this.writeH((short) 4103);
      this.writeH((short) 0);
      this.writeC((byte) this.PlayersCount);
      for (int index = 0; index < 16; ++index)
      {
        Slot slot = this.room._slots[index];
        if (slot.state >= SlotState.READY && slot._equip != null)
        {
          Account playerBySlot = this.room.getPlayerBySlot(slot);
          if (playerBySlot != null && playerBySlot._slotId == index)
            this.writeD((uint) playerBySlot.player_id);
        }
      }
      this.writeC((byte) 16);
      for (int index = 0; index < 16; ++index)
        this.writeC((byte) 1);
      this.writeC((byte) this.PlayersCount);
      this.writeB(this.Data);
      this.writeC((byte) this.room.mapId);
      this.writeC((byte) this.room.rule);
      this.writeC(this.room.stage);
      this.writeC((byte) this.room.room_type);
    }
  }
}
