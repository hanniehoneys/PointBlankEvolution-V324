using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_USER_LEAVE_ACK : SendPacket
  {
    private int erro;

    public PROTOCOL_BASE_USER_LEAVE_ACK(int erro)
    {
      this.erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 537);
      this.writeH((short) 0);
      this.writeD(this.erro);
    }
  }
}
