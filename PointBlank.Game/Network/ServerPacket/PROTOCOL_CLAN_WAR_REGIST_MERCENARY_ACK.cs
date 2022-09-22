using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK : SendPacket
  {
    private Match m;

    public PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK(Match m)
    {
      this.m = m;
    }

    public override void write()
    {
      this.writeH((short) 1552);
      this.writeH((short) this.m.getServerInfo());
      this.writeC((byte) this.m._state);
      this.writeC((byte) this.m.friendId);
      this.writeC((byte) this.m.formação);
      this.writeC((byte) this.m.getCountPlayers());
      this.writeD(this.m._leader);
      this.writeC((byte) 0);
      for (int index = 0; index < this.m._slots.Length; ++index)
      {
        SlotMatch slot = this.m._slots[index];
        Account playerBySlot = this.m.getPlayerBySlot(slot);
        if (playerBySlot != null)
        {
          this.writeC((byte) playerBySlot._rank);
          this.writeS(playerBySlot.player_name, 33);
          this.writeQ(slot._playerId);
          this.writeC((byte) slot.state);
        }
        else
          this.writeB(new byte[43]);
      }
    }
  }
}
