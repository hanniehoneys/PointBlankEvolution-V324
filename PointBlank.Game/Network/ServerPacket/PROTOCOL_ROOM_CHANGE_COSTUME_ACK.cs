using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_CHANGE_COSTUME_ACK : SendPacket
  {
    private Slot Slot;

    public PROTOCOL_ROOM_CHANGE_COSTUME_ACK(Slot Slot)
    {
      this.Slot = Slot;
    }

    public override void write()
    {
      this.writeH((short) 3932);
      this.writeD(this.Slot._id);
      this.writeC((byte) this.Slot.Costume);
    }
  }
}
