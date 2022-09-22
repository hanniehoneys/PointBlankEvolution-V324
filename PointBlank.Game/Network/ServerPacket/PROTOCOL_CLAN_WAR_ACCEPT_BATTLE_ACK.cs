using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_ACCEPT_BATTLE_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_CLAN_WAR_ACCEPT_BATTLE_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1559);
      this.writeD(this._erro);
    }
  }
}
