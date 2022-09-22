using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_GET_COLOR_NICK_ACK : SendPacket
  {
    private int Slot;
    private int Color;

    public PROTOCOL_ROOM_GET_COLOR_NICK_ACK(int Slot, int Color)
    {
      this.Slot = Slot;
      this.Color = Color;
    }

    public override void write()
    {
      this.writeH((short) 3892);
      this.writeD(this.Slot);
      this.writeC((byte) this.Color);
    }
  }
}
