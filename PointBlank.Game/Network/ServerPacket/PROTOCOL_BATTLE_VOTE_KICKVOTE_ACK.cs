using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_VOTE_KICKVOTE_ACK : SendPacket
  {
    private uint erro;

    public PROTOCOL_BATTLE_VOTE_KICKVOTE_ACK(uint erro)
    {
      this.erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 3401);
      this.writeD(this.erro);
    }
  }
}
