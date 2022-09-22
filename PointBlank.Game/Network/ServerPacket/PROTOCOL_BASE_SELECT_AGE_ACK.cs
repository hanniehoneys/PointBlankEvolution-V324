using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_SELECT_AGE_ACK : SendPacket
  {
    private int Error;
    private int Age;

    public PROTOCOL_BASE_SELECT_AGE_ACK(int Error, int Age)
    {
      this.Error = Error;
      this.Age = Age;
    }

    public override void write()
    {
      this.writeH((short) 3096);
      this.writeD(this.Error);
      if (this.Error != 0)
        return;
      this.writeC((byte) this.Age);
    }
  }
}
