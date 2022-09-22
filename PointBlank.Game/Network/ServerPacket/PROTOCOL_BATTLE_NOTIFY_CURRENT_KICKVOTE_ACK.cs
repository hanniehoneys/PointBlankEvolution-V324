using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_NOTIFY_CURRENT_KICKVOTE_ACK : SendPacket
  {
    private VoteKick _k;

    public PROTOCOL_BATTLE_NOTIFY_CURRENT_KICKVOTE_ACK(VoteKick vote)
    {
      this._k = vote;
    }

    public override void write()
    {
      this.writeH((short) 3407);
      this.writeC((byte) this._k.kikar);
      this.writeC((byte) this._k.deixar);
    }
  }
}
