using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using System.Net;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_CHANGE_ROOMINFO_ACK : SendPacket
  {
    private Room room;
    private bool isBotMode;

    public PROTOCOL_ROOM_CHANGE_ROOMINFO_ACK(Room r)
    {
      this.room = r;
      if (this.room == null)
        return;
      this.isBotMode = this.room.isBotMode();
    }

    public PROTOCOL_ROOM_CHANGE_ROOMINFO_ACK(Room r, bool isBotMode)
    {
      this.room = r;
      this.isBotMode = isBotMode;
    }

    public override void write()
    {
      Account leader = this.room.getLeader();
      if (this.room == null)
        return;
      this.writeH((short) 3857);
      this.writeD(this.room._roomId);
      this.writeUnicode(this.room.name, 46);
      this.writeC((byte) this.room.mapId);
      this.writeC((byte) this.room.rule);
      this.writeC(this.room.stage);
      this.writeC((byte) this.room.room_type);
      this.writeC((byte) this.room._state);
      this.writeC((byte) this.room.getAllPlayers().Count);
      this.writeC((byte) this.room.getSlotCount());
      this.writeC((byte) this.room._ping);
      this.writeH((ushort) this.room.weaponsFlag);
      this.writeD(this.room.getFlag());
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeUnicode(leader != null ? leader.player_name : "", 66);
      this.writeD(this.room.killtime);
      this.writeC(this.room.Limit);
      this.writeC(this.room.WatchRuleFlag);
      this.writeH(this.room.BalanceType);
      this.writeB(new byte[16]);
      this.writeIP(leader.PublicIP == null ? IPAddress.Parse("127.0.0.1") : leader.PublicIP);
      if (!this.isBotMode)
        return;
      this.writeC(this.room.aiCount);
      this.writeC(this.room.aiLevel);
    }
  }
}
