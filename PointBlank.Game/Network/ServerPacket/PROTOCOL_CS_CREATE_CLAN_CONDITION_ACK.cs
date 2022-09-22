using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CREATE_CLAN_CONDITION_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 1937);
      this.writeC((byte) GameConfig.minCreateRank);
      this.writeD(GameConfig.minCreateGold);
    }
  }
}
