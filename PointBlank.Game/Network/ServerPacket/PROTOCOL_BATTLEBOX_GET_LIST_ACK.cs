using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLEBOX_GET_LIST_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 7426);
      this.writeD(1);
      this.writeD(1);
      this.writeD(0);
      this.writeD(2800001);
      this.writeH((short) 0);
      this.writeC((byte) 0);
      this.writeD(585);
    }
  }
}
