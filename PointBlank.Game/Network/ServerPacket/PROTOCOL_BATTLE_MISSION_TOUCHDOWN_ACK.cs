using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_MISSION_TOUCHDOWN_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Room r;
    private Slot slot;

    public PROTOCOL_BATTLE_MISSION_TOUCHDOWN_ACK(PointBlank.Game.Data.Model.Room room, Slot slot)
    {
      this.r = room;
      this.slot = slot;
    }

    public override void write()
    {
      this.writeH((short) 4155);
      this.writeH((ushort) this.r.red_dino);
      this.writeH((ushort) this.r.blue_dino);
      this.writeD(this.slot._id);
      this.writeH((short) this.slot.passSequence);
    }
  }
}
