using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_TIMEOUTCLIENT_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 4120);
    }
  }
}
