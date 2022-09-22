using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_GET_RANK_ACK : SendPacket
  {
    private int Slot;
    private int Rank;

    public PROTOCOL_ROOM_GET_RANK_ACK(int Slot, int Rank)
    {
      this.Slot = Slot;
      this.Rank = Rank;
    }

    public override void write()
    {
      this.writeH((short) 3890);
      this.writeD(this.Slot);
      this.writeD(this.Rank);
    }
  }
}
