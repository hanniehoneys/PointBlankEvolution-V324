using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_NOTIFY_KICKVOTE_RESULT_ACK : SendPacket
  {
    private VoteKick vote;
    private uint erro;

    public PROTOCOL_BATTLE_NOTIFY_KICKVOTE_RESULT_ACK(uint erro, VoteKick vote)
    {
      this.erro = erro;
      this.vote = vote;
    }

    public override void write()
    {
      this.writeH((short) 3403);
      this.writeC((byte) this.vote.victimIdx);
      this.writeC((byte) this.vote.kikar);
      this.writeC((byte) this.vote.deixar);
      this.writeD(this.erro);
    }
  }
}
