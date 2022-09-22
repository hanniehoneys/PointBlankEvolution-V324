using PointBlank.Core.Network;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_MEMBER_CONTEXT_ACK : SendPacket
  {
    private int erro;
    private int playersCount;

    public PROTOCOL_CS_MEMBER_CONTEXT_ACK(int erro, int playersCount)
    {
      this.erro = erro;
      this.playersCount = playersCount;
    }

    public PROTOCOL_CS_MEMBER_CONTEXT_ACK(int erro)
    {
      this.erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1827);
      this.writeD(this.erro);
      if (this.erro != 0)
        return;
      this.writeC((byte) this.playersCount);
      this.writeC((byte) 14);
      this.writeC((byte) Math.Ceiling((double) this.playersCount / 14.0));
      this.writeD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
    }
  }
}
