using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CLAN_MATCH_RESULT_LIST_ACK : SendPacket
  {
    private List<Match> _c;
    private int _erro;

    public PROTOCOL_CS_CLAN_MATCH_RESULT_LIST_ACK(int erro, List<Match> c)
    {
      this._erro = erro;
      this._c = c;
    }

    public override void write()
    {
      this.writeH((short) 1957);
      this.writeC(this._erro == 0 ? (byte) this._c.Count : (byte) this._erro);
      if (this._erro > 0 || this._c.Count == 0)
        return;
      this.writeC((byte) 1);
      this.writeC((byte) 0);
      this.writeC((byte) this._c.Count);
      for (int index = 0; index < this._c.Count; ++index)
      {
        Match match = this._c[index];
        this.writeH((short) match._matchId);
        this.writeH((ushort) match.getServerInfo());
        this.writeH((ushort) match.getServerInfo());
        this.writeC((byte) match._state);
        this.writeC((byte) match.friendId);
        this.writeC((byte) match.formação);
        this.writeC((byte) match.getCountPlayers());
        this.writeC((byte) 0);
        this.writeD(match._leader);
        Account leader = match.getLeader();
        if (leader != null)
        {
          this.writeC((byte) leader._rank);
          this.writeUnicode(leader.player_name, 66);
          this.writeQ(leader.player_id);
          this.writeC((byte) match._slots[match._leader].state);
        }
        else
          this.writeB(new byte[76]);
      }
    }
  }
}
