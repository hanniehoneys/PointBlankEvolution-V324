using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SHOP_PLUS_POINT_ACK : SendPacket
  {
    private int gp;
    private int _gpIncrease;
    private int type;

    public PROTOCOL_SHOP_PLUS_POINT_ACK(int increase, int gold, int type)
    {
      this._gpIncrease = increase;
      this.gp = gold;
      this.type = type;
    }

    public override void write()
    {
      this.writeH((short) 1072);
      this.writeH((short) 0);
      this.writeC((byte) this.type);
      this.writeD(this.gp);
      this.writeD(this._gpIncrease);
    }
  }
}
