using PointBlank.Core.Network;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CLIENT_CLAN_CONTEXT_ACK : SendPacket
  {
    private int clansCount;

    public PROTOCOL_CS_CLIENT_CLAN_CONTEXT_ACK(int count)
    {
      this.clansCount = count;
    }

    public override void write()
    {
      this.writeH((short) 1800);
      this.writeD(this.clansCount);
      this.writeC((byte) 15);
      this.writeH((ushort) Math.Ceiling((double) this.clansCount / 15.0));
      this.writeD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
    }
  }
}
