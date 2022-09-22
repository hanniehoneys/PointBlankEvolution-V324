using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_INVITE_LOBBY_USER_LIST_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_ROOM_INVITE_LOBBY_USER_LIST_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 3930);
      this.writeD(this._erro);
    }
  }
}
