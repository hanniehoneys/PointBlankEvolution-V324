using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CHECK_MARK_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_CS_CHECK_MARK_ACK(uint er)
    {
      this._erro = er;
    }

    public override void write()
    {
      this.writeH((short) 1881);
      this.writeD(this._erro);
    }
  }
}
