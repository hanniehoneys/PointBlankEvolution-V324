using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_MISSION_TOUCHDOWN_COUNT_ACK : SendPacket
  {
    private Room _r;

    public PROTOCOL_BATTLE_MISSION_TOUCHDOWN_COUNT_ACK(Room r)
    {
      this._r = r;
    }

    public override void write()
    {
      this.writeH((short) 4159);
      this.writeH((ushort) this._r.red_dino);
      this.writeH((ushort) this._r.blue_dino);
    }
  }
}
