using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_ACK : SendPacket
  {
    private Room room;

    public PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_ACK(Room room)
    {
      this.room = room;
    }

    public override void write()
    {
      this.writeH((short) 4157);
      this.writeH((ushort) this.room.Bar1);
      this.writeH((ushort) this.room.Bar2);
      for (int index = 0; index < 16; ++index)
        this.writeH(this.room._slots[index].damageBar1);
      for (int index = 0; index < 16; ++index)
        this.writeH(this.room._slots[index].damageBar2);
    }
  }
}
