using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_INVENTORY_LEAVE_ACK : SendPacket
  {
    private int erro;

    public PROTOCOL_INVENTORY_LEAVE_ACK(int erro)
    {
      this.erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 3332);
      this.writeH((short) 0);
      this.writeD(this.erro);
    }
  }
}
