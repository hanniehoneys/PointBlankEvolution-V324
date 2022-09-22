using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_REQUEST_CONTEXT_ACK : SendPacket
  {
    private uint _erro;
    private int invites;

    public PROTOCOL_CS_REQUEST_CONTEXT_ACK(int clanId)
    {
      if (clanId > 0)
        this.invites = PlayerManager.getRequestCount(clanId);
      else
        this._erro = uint.MaxValue;
    }

    public override void write()
    {
      this.writeH((short) 1841);
      this.writeD(this._erro);
      if (this._erro != 0U)
        return;
      this.writeC((byte) this.invites);
      this.writeC((byte) 13);
      this.writeC((byte) Math.Ceiling((double) this.invites / 13.0));
      this.writeD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
    }
  }
}
