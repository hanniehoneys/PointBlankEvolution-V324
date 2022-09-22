using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_CREATE_TEAM_ACK : SendPacket
  {
    private uint _erro;
    private Match m;

    public PROTOCOL_CLAN_WAR_CREATE_TEAM_ACK(uint erro, Match mt = null)
    {
      this._erro = erro;
      this.m = mt;
    }

    public override void write()
    {
      this.writeH((short) 1547);
      this.writeD(this._erro);
      if (this._erro != 0U)
        return;
      this.writeH((short) this.m._matchId);
      this.writeH((short) this.m.getServerInfo());
      this.writeH((short) this.m.getServerInfo());
      this.writeC((byte) this.m._state);
      this.writeC((byte) this.m.friendId);
      this.writeC((byte) this.m.formação);
      this.writeC((byte) this.m.getCountPlayers());
      this.writeD(this.m._leader);
      this.writeC((byte) 0);
    }
  }
}
