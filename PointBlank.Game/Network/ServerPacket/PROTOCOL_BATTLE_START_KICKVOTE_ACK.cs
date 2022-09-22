using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_START_KICKVOTE_ACK : SendPacket
  {
    private VoteKick vote;

    public PROTOCOL_BATTLE_START_KICKVOTE_ACK(VoteKick vote)
    {
      this.vote = vote;
    }

    public override void write()
    {
      this.writeH((short) 3399);
      this.writeC((byte) this.vote.creatorIdx);
      this.writeC((byte) this.vote.victimIdx);
      this.writeC((byte) this.vote.motive);
    }
  }
}
