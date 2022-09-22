using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_ACK : SendPacket
  {
    private Room _room;

    public PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_ACK(Room room)
    {
      this._room = room;
    }

    public override void write()
    {
      this.writeH((short) 4143);
      this.writeH((ushort) this._room.Bar1);
      this.writeH((ushort) this._room.Bar2);
      for (int index = 0; index < 16; ++index)
        this.writeH(this._room._slots[index].damageBar1);
    }
  }
}
