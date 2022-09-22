using PointBlank.Core.Network;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_MATCH_TEAM_COUNT_ACK : SendPacket
  {
    private int count;

    public PROTOCOL_CLAN_WAR_MATCH_TEAM_COUNT_ACK(int count)
    {
      this.count = count;
    }

    public override void write()
    {
      this.writeH((short) 6915);
      this.writeH((short) this.count);
      this.writeC((byte) 13);
      this.writeH((short) Math.Ceiling((double) this.count / 13.0));
    }
  }
}
