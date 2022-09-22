using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_USER_ENTER_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_BASE_USER_ENTER_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 539);
      this.writeH((short) 0);
      this.writeD(0);
    }
  }
}
