using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_SUGGEST_KICKVOTE_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_BATTLE_SUGGEST_KICKVOTE_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 3397);
      this.writeD(this._erro);
    }
  }
}
