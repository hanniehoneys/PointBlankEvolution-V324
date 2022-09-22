using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_REPLACE_PERSONMAX_ACK : SendPacket
  {
    private int _max;

    public PROTOCOL_CS_REPLACE_PERSONMAX_ACK(int max)
    {
      this._max = max;
    }

    public override void write()
    {
      this.writeH((short) 1897);
      this.writeD(0);
      this.writeC((byte) this._max);
    }
  }
}
