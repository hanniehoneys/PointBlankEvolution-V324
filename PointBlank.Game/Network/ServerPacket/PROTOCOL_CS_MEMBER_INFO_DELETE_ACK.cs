using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_MEMBER_INFO_DELETE_ACK : SendPacket
  {
    private long _pId;

    public PROTOCOL_CS_MEMBER_INFO_DELETE_ACK(long pId)
    {
      this._pId = pId;
    }

    public override void write()
    {
      this.writeH((short) 1873);
      this.writeQ(this._pId);
    }
  }
}
