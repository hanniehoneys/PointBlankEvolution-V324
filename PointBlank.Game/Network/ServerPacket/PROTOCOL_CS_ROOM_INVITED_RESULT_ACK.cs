using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_ROOM_INVITED_RESULT_ACK : SendPacket
  {
    private long _pId;

    public PROTOCOL_CS_ROOM_INVITED_RESULT_ACK(long pId)
    {
      this._pId = pId;
    }

    public override void write()
    {
      this.writeH((short) 1903);
      this.writeQ(this._pId);
    }
  }
}
