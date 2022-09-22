using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_GET_NICKNAME_ACK : SendPacket
  {
    private int slotIdx;
    private int color;
    private string name;

    public PROTOCOL_ROOM_GET_NICKNAME_ACK(int slot, string name, int color)
    {
      this.slotIdx = slot;
      this.name = name;
      this.color = color;
    }

    public override void write()
    {
      this.writeH((short) 3855);
      this.writeD(this.slotIdx);
      if (this.slotIdx < 0)
        return;
      this.writeUnicode(this.name, 33);
      this.writeC((byte) this.color);
    }
  }
}
