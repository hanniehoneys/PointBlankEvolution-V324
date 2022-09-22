using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CLIENT_CLAN_LIST_ACK : SendPacket
  {
    private uint _page;
    private int _count;
    private byte[] _array;

    public PROTOCOL_CS_CLIENT_CLAN_LIST_ACK(uint page, int count, byte[] array)
    {
      this._page = page;
      this._count = count;
      this._array = array;
    }

    public override void write()
    {
      this.writeH((short) 1798);
      this.writeD(this._page);
      this.writeC((byte) this._count);
      this.writeB(this._array);
    }
  }
}
