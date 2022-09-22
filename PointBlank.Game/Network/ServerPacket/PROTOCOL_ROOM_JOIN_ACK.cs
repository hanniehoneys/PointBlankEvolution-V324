using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using System.Collections.Generic;
using System.Net;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_JOIN_ACK : SendPacket
  {
    private uint erro;
    private PointBlank.Game.Data.Model.Room room;
    private int slotId;
    private PointBlank.Game.Data.Model.Account leader;
    private PointBlank.Game.Data.Model.Account player;

    public PROTOCOL_ROOM_JOIN_ACK(uint erro, PointBlank.Game.Data.Model.Account player = null, PointBlank.Game.Data.Model.Account leader = null)
    {
      this.erro = erro;
      this.player = player;
      if (player == null)
        return;
      this.slotId = player._slotId;
      this.room = player._room;
      this.leader = leader;
    }

    public override void write()
    {
      this.writeH((short) 3844);
      this.writeH((short) 0);
      if (this.erro == 0U && this.room != null && this.leader != null)
      {
        lock (this.room._slots)
          this.WriteData();
      }
      else
        this.writeD(this.erro);
    }

    private void WriteData()
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = this.room.getAllPlayers();
      this.writeD(0);
      this.writeC((byte) 16);
      for (int slotIdx = 0; slotIdx < 16; ++slotIdx)
        this.writeC((byte) (this.room.getSlot(slotIdx)._id % 2));
      this.writeC((byte) 16);
      for (int index = 0; index < 16; ++index)
      {
        PointBlank.Core.Models.Room.Slot slot = this.room._slots[index];
        PointBlank.Game.Data.Model.Account playerBySlot = this.room.getPlayerBySlot(slot);
        if (playerBySlot != null)
        {
          PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(playerBySlot.clanId);
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
          this.writeUnicode(clan._name, 34);
          this.writeC((byte) 0);
          this.writeC((byte) 210);
          this.writeC((byte) slot._id);
          this.writeUnicode(playerBySlot.player_name, 66);
          this.writeC((byte) playerBySlot.name_color);
          this.writeC((byte) playerBySlot._bonus.muzzle);
          this.writeC((byte) 0);
          this.writeC(byte.MaxValue);
          this.writeC(byte.MaxValue);
        }
        else
        {
          this.writeC((byte) slot.state);
          this.writeB(new byte[10]);
          this.writeD(uint.MaxValue);
          this.writeB(new byte[46]);
          this.writeC((byte) 210);
          this.writeC((byte) slot._id);
          this.writeB(new byte[69]);
          this.writeC(byte.MaxValue);
          this.writeC(byte.MaxValue);
        }
      }
      this.writeC(this.room.aiType);
      this.writeC(this.room._state > RoomState.Ready ? this.room.IngameAiLevel : this.room.aiLevel);
      this.writeC(this.room.aiCount);
      this.writeC((byte) allPlayers.Count);
      this.writeC((byte) this.room._leader);
      this.writeC((byte) this.room.countdown.getTimeLeft());
      this.writeC((byte) 4);
      this.writeS(this.room.password, 4);
      this.writeB(new byte[17]);
      this.writeUnicode(this.leader.player_name, 66);
      this.writeD(this.room.killtime);
      this.writeC(this.room.Limit);
      this.writeC(this.room.WatchRuleFlag);
      this.writeH(this.room.BalanceType);
      this.writeB(new byte[16]);
      this.writeIP(this.room.getLeader().PublicIP == null ? IPAddress.Parse("127.0.0.1") : this.room.getLeader().PublicIP);
      this.writeD(this.room._roomId);
      this.writeUnicode(this.room.name, 46);
      this.writeC((byte) this.room.mapId);
      this.writeC((byte) this.room.rule);
      this.writeC(this.room.stage);
      this.writeC((byte) this.room.room_type);
      this.writeC((byte) this.room._state);
      this.writeC((byte) allPlayers.Count);
      this.writeC((byte) this.room.getSlotCount());
      this.writeC((byte) this.room._ping);
      this.writeH((ushort) this.room.weaponsFlag);
      this.writeD(this.room.getFlag());
      this.writeH((short) 0);
      this.writeC((byte) this.slotId);
    }
  }
}
