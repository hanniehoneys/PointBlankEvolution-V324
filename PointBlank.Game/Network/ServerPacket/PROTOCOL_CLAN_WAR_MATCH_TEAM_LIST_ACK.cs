using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_MATCH_TEAM_LIST_ACK : SendPacket
  {
    private List<Match> matchs;
    private int myMatchIdx;
    private int _page;
    private int MatchCount;

    public PROTOCOL_CLAN_WAR_MATCH_TEAM_LIST_ACK(int page, List<Match> matchs, int matchId)
    {
      this._page = page;
      this.myMatchIdx = matchId;
      this.MatchCount = matchs.Count - 1;
      this.matchs = matchs;
    }

    public override void write()
    {
      this.writeH((short) 1545);
      this.writeH((ushort) this.MatchCount);
      if (this.MatchCount == 0)
        return;
      this.writeH((short) 1);
      this.writeH((short) 0);
      this.writeC((byte) this.MatchCount);
      for (int index = 0; index < this.matchs.Count; ++index)
      {
        Match match = this.matchs[index];
        if (match._matchId != this.myMatchIdx)
        {
          this.writeH((short) match._matchId);
          this.writeH((short) match.getServerInfo());
          this.writeH((short) match.getServerInfo());
          this.writeC((byte) match._state);
          this.writeC((byte) match.friendId);
          this.writeC((byte) match.formação);
          this.writeC((byte) match.getCountPlayers());
          this.writeD(match._leader);
          this.writeC((byte) 0);
          this.writeD(match.clan._id);
          this.writeC((byte) match.clan._rank);
          this.writeD(match.clan._logo);
          this.writeS(match.clan._name, 17);
          this.writeT(match.clan._pontos);
          this.writeC((byte) match.clan._name_color);
          Account leader = match.getLeader();
          if (leader != null)
          {
            this.writeC((byte) leader._rank);
            this.writeS(leader.player_name, 33);
            this.writeQ(leader.player_id);
            this.writeC((byte) match._slots[match._leader].state);
          }
          else
            this.writeB(new byte[43]);
        }
      }
    }
  }
}
