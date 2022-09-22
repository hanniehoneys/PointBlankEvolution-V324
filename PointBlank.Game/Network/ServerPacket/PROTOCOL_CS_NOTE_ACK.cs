using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_NOTE_ACK : SendPacket
  {
    private int playersCount;

    public PROTOCOL_CS_NOTE_ACK(int count)
    {
      this.playersCount = count;
    }

    public override void write()
    {
      this.writeH((short) 1917);
      this.writeD(this.playersCount);
    }
  }
}
