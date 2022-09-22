using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_SERVER_MESSAGE_ERROR_ACK : SendPacket
  {
    private uint _er;

    public PROTOCOL_SERVER_MESSAGE_ERROR_ACK(uint er)
    {
      this._er = er;
    }

    public override void write()
    {
      this.writeH((short) 2566);
      this.writeD(this._er);
    }
  }
}
