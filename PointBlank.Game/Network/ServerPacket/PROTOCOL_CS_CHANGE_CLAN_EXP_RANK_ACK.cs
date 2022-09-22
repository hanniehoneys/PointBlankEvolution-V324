using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CHANGE_CLAN_EXP_RANK_ACK : SendPacket
  {
    private int Exp;

    public PROTOCOL_CS_CHANGE_CLAN_EXP_RANK_ACK(int Exp)
    {
      this.Exp = Exp;
    }

    public override void write()
    {
      this.writeH((short) 1904);
      this.writeC((byte) this.Exp);
    }
  }
}
