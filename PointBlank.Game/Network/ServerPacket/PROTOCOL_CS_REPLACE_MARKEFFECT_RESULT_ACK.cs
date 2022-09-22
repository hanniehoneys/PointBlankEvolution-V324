using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_REPLACE_MARKEFFECT_RESULT_ACK : SendPacket
  {
    private int Effect;

    public PROTOCOL_CS_REPLACE_MARKEFFECT_RESULT_ACK(int Effect)
    {
      this.Effect = Effect;
    }

    public override void write()
    {
      this.writeH((short) 1994);
      this.writeC((byte) this.Effect);
    }
  }
}
