using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_RESULT_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 6964);
    }
  }
}
