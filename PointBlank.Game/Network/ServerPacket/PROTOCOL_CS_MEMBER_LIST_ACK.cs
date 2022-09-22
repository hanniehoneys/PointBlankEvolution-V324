using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_MEMBER_LIST_ACK : SendPacket
  {
    private byte[] array;
    private int erro;
    private int page;
    private int count;

    public PROTOCOL_CS_MEMBER_LIST_ACK(int page, int count, byte[] array)
    {
      this.page = page;
      this.count = count;
      this.array = array;
    }

    public PROTOCOL_CS_MEMBER_LIST_ACK(int erro)
    {
      this.erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1829);
      this.writeD(this.erro);
      if (this.erro < 0)
        return;
      this.writeC((byte) this.page);
      this.writeC((byte) this.count);
      this.writeB(this.array);
    }
  }
}
