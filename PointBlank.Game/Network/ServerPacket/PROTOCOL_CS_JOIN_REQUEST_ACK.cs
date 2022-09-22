using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_JOIN_REQUEST_ACK : SendPacket
  {
    private int _clanId;
    private uint _erro;

    public PROTOCOL_CS_JOIN_REQUEST_ACK(uint erro, int clanId)
    {
      this._erro = erro;
      this._clanId = clanId;
    }

    public override void write()
    {
      this.writeH((short) 1837);
      this.writeD(this._erro);
      if (this._erro != 0U)
        return;
      this.writeD(this._clanId);
    }
  }
}
