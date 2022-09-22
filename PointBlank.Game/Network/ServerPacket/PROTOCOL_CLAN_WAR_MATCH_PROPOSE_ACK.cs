using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_MATCH_PROPOSE_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_CLAN_WAR_MATCH_PROPOSE_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1554);
      this.writeD(this._erro);
    }
  }
}
