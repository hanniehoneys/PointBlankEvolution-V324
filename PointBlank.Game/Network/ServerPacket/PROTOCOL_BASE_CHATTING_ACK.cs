using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_CHATTING_ACK : SendPacket
  {
    private int erro;
    private int banTime;

    public PROTOCOL_BASE_CHATTING_ACK(int erro, int time = 0)
    {
      this.erro = erro;
      this.banTime = time;
    }

    public override void write()
    {
      this.writeH((short) 2628);
      this.writeC((byte) this.erro);
      if (this.erro <= 0)
        return;
      this.writeD(this.banTime);
    }
  }
}
