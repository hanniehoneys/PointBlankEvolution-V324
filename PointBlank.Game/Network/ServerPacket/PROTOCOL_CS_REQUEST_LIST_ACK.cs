using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_REQUEST_LIST_ACK : SendPacket
  {
    private int erro;
    private int page;
    private int count;
    private byte[] array;

    public PROTOCOL_CS_REQUEST_LIST_ACK(int erro, int count, int page, byte[] array)
    {
      this.erro = erro;
      this.count = count;
      this.page = page;
      this.array = array;
    }

    public PROTOCOL_CS_REQUEST_LIST_ACK(int erro)
    {
      this.erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1843);
      this.writeD(this.erro);
      if (this.erro < 0)
        return;
      this.writeC((byte) this.page);
      this.writeC((byte) this.count);
      this.writeB(this.array);
    }
  }
}
