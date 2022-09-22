using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_CHANGE_MAX_PER_ACK : SendPacket
  {
    public Match mt;
    public Account p;

    public PROTOCOL_CLAN_WAR_CHANGE_MAX_PER_ACK(Match match, Account p)
    {
      this.mt = match;
      this.p = p;
    }

    public override void write()
    {
      this.writeH((short) 1555);
      this.writeH((short) this.mt._matchId);
      this.writeH((ushort) this.mt.getServerInfo());
      this.writeH((ushort) this.mt.getServerInfo());
      this.writeC((byte) this.mt._state);
      this.writeC((byte) this.mt.friendId);
      this.writeC((byte) this.mt.formação);
      this.writeC((byte) this.mt.getCountPlayers());
      this.writeD(this.mt._leader);
      this.writeC((byte) 0);
      this.writeD(this.mt.clan._id);
      this.writeC((byte) this.mt.clan._rank);
      this.writeD(this.mt.clan._logo);
      this.writeS(this.mt.clan._name, 17);
      this.writeT(this.mt.clan._pontos);
      this.writeC((byte) this.mt.clan._name_color);
      if (this.p != null)
      {
        this.writeC((byte) this.p._rank);
        this.writeS(this.p.player_name, 33);
        this.writeQ(this.p.player_id);
        this.writeC((byte) this.mt._slots[this.mt._leader].state);
      }
      else
        this.writeB(new byte[43]);
    }
  }
}
