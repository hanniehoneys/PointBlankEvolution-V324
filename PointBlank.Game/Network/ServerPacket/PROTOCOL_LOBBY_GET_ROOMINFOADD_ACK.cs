using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_LOBBY_GET_ROOMINFOADD_ACK : SendPacket
  {
    private Room room;
    private Account leader;

    public PROTOCOL_LOBBY_GET_ROOMINFOADD_ACK(Room room, Account leader)
    {
      this.room = room;
      this.leader = leader;
    }

    public override void write()
    {
      if (this.room == null || this.leader == null)
        return;
      this.writeH((short) 3084);
      this.writeC((byte) 0);
      try
      {
        this.writeUnicode(this.leader.player_name, 66);
        this.writeC((byte) this.room.killtime);
        this.writeC((byte) (this.room.rounds - 1));
        this.writeH((ushort) this.room.getInBattleTime());
        this.writeC(this.room.Limit);
        this.writeC(this.room.WatchRuleFlag);
        this.writeH(this.room.BalanceType);
        this.writeB(new byte[16]);
        this.writeIP(this.leader.PublicIP);
      }
      catch (Exception ex)
      {
        this.writeC((byte) 0);
        this.writeUnicode("", 66);
        this.writeB(new byte[28]);
        Logger.warning("PROTOCOL_LOBBY_GET_ROOMINFOADD_ACK: " + ex.ToString());
      }
    }
  }
}
