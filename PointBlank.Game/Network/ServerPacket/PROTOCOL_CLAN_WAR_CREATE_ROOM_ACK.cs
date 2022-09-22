using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_CREATE_ROOM_ACK : SendPacket
  {
    public Match _mt;

    public PROTOCOL_CLAN_WAR_CREATE_ROOM_ACK(Match match)
    {
      this._mt = match;
    }

    public override void write()
    {
      this.writeH((short) 1564);
      this.writeH((short) this._mt._matchId);
      this.writeD(this._mt.getServerInfo());
      this.writeH((short) this._mt.getServerInfo());
      this.writeC((byte) 10);
    }
  }
}
