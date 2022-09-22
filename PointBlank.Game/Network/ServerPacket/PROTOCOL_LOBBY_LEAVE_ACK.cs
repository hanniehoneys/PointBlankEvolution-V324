using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_LOBBY_LEAVE_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_LOBBY_LEAVE_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 3076);
      this.writeD(this._erro);
    }
  }
}
