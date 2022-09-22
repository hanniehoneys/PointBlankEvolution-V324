using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_RANK_UP_ACK : SendPacket
  {
    private int _rank;
    private int _allExp;

    public PROTOCOL_BASE_RANK_UP_ACK(int rank, int allExp)
    {
      this._rank = rank;
      this._allExp = allExp;
    }

    public override void write()
    {
      this.writeH((short) 551);
      this.writeD(this._rank);
      this.writeD(this._rank);
      this.writeD(this._allExp);
    }
  }
}
