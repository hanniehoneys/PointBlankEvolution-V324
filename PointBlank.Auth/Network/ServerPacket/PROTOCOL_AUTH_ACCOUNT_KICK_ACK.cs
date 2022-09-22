using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_AUTH_ACCOUNT_KICK_ACK : SendPacket
  {
    private int _type;

    public PROTOCOL_AUTH_ACCOUNT_KICK_ACK(int type)
    {
      this._type = type;
    }

    public override void write()
    {
      this.writeH((short) 998);
      this.writeC((byte) this._type);
    }
  }
}
