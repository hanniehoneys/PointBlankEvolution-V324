using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_FRIEND_INSERT_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_AUTH_FRIEND_INSERT_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 795);
      this.writeD(this._erro);
    }
  }
}
