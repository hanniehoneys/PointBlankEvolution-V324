using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CANCEL_REQUEST_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_CS_CANCEL_REQUEST_ACK(uint error)
    {
      this._erro = error;
    }

    public override void write()
    {
      this.writeH((short) 1839);
      this.writeD(this._erro);
    }
  }
}
