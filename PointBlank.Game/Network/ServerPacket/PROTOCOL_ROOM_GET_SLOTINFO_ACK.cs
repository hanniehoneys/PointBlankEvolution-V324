using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_GET_SLOTINFO_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Room room;

    public PROTOCOL_ROOM_GET_SLOTINFO_ACK(PointBlank.Game.Data.Model.Room r)
    {
      this.room = r;
    }

    public override void write()
    {
      try
      {
        if (this.room == null)
          return;
        this.writeH((short) 3848);
        if (this.room.getLeader() == null)
          this.room.setNewLeader(-1, 0, this.room._leader, false);
        if (this.room.getLeader() == null)
          return;
        this.writeC((byte) this.room._leader);
        for (int index = 0; index < 16; ++index)
        {
          Slot slot = this.room._slots[index];
          PointBlank.Game.Data.Model.Account playerBySlot = this.room.getPlayerBySlot(slot);
          if (playerBySlot != null)
          {
            uint effects = (uint) playerBySlot.effects;
            PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(playerBySlot.clanId);
            this.writeH((short) 26);
            this.writeC((byte) slot.state);
            this.writeC((byte) playerBySlot.getRank());
            this.writeD(clan._id);
            this.writeD(playerBySlot.clanAccess);
            this.writeC((byte) clan._rank);
            this.writeD(clan._logo);
            this.writeC((byte) playerBySlot.pc_cafe);
            this.writeC((byte) playerBySlot.tourneyLevel);
            this.writeD((uint) playerBySlot.effects);
            this.writeD(0);
            this.writeC((byte) clan.effect);
            this.writeH((ushort) (clan._name.Length * 2));
            this.writeUnicode(clan._name, false);
            this.writeC((byte) 0);
            this.writeC((byte) 210);
          }
          else
          {
            this.writeH((short) 26);
            this.writeC((byte) slot.state);
            this.writeB(new byte[10]);
            this.writeD(uint.MaxValue);
            this.writeB(new byte[15]);
          }
        }
        if (this.room.room_type == RoomType.FreeForAll)
        {
          for (int index = 0; index < 16; ++index)
            this.writeC((byte) this.room._slots[index].Costume);
        }
        else
        {
          for (int index = 0; index < 16; ++index)
            this.writeC((byte) (this.room._slots[index]._id % 2));
        }
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_ROOM_GET_SLOTINFO_ACK: " + ex.ToString());
      }
    }
  }
}
