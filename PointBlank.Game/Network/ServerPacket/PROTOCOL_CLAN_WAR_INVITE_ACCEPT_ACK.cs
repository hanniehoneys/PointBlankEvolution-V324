using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_INVITE_ACCEPT_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_CLAN_WAR_INVITE_ACCEPT_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1560);
      this.writeD(this._erro);
    }
  }
}
