using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_COMMISSION_MASTER_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_CS_COMMISSION_MASTER_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1858);
      this.writeD(this._erro);
    }
  }
}
