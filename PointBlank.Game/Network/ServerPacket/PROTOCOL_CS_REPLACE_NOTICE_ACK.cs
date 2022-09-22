using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_REPLACE_NOTICE_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_CS_REPLACE_NOTICE_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1883);
      this.writeD(this._erro);
    }
  }
}
