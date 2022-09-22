using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_USE_ITEM_CHECK_NICK_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_AUTH_USE_ITEM_CHECK_NICK_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1062);
      this.writeD(this._erro);
    }
  }
}
