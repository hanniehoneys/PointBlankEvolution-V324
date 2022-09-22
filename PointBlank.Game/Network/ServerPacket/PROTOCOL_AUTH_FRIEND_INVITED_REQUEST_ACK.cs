using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_FRIEND_INVITED_REQUEST_ACK : SendPacket
  {
    private int _idx;

    public PROTOCOL_AUTH_FRIEND_INVITED_REQUEST_ACK(int idx)
    {
      this._idx = idx;
    }

    public override void write()
    {
      this.writeH((short) 789);
      this.writeC((byte) this._idx);
    }
  }
}
