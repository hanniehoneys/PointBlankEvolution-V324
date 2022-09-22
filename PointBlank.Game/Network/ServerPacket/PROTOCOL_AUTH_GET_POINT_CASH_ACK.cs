using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_GET_POINT_CASH_ACK : SendPacket
  {
    private int erro;
    private int gold;
    private int cash;

    public PROTOCOL_AUTH_GET_POINT_CASH_ACK(int erro, int gold = 0, int cash = 0)
    {
      this.erro = erro;
      this.gold = gold;
      this.cash = cash;
    }

    public override void write()
    {
      this.writeH((short) 1058);
      this.writeD(this.erro);
      if (this.erro >= 0)
      {
        this.writeD(this.gold);
        this.writeD(this.cash);
      }
      this.writeD(0);
    }
  }
}
