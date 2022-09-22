using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_HOLE_CHECK_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 4098);
      this.writeD(0);
    }
  }
}
