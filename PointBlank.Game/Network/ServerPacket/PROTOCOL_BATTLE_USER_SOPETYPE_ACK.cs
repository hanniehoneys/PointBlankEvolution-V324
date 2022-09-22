using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_USER_SOPETYPE_ACK : SendPacket
  {
    private Account Player;

    public PROTOCOL_BATTLE_USER_SOPETYPE_ACK(Account Player)
    {
      this.Player = Player;
    }

    public override void write()
    {
      this.writeH((short) 4253);
      this.writeD(this.Player._slotId);
      this.writeC((byte) this.Player.Sight);
    }
  }
}
