using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CLOSE_CLAN_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_CS_CLOSE_CLAN_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1833);
      this.writeD(this._erro);
    }
  }
}
