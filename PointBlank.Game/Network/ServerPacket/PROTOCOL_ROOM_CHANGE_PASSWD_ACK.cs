using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_CHANGE_PASSWD_ACK : SendPacket
  {
    private string _pass;

    public PROTOCOL_ROOM_CHANGE_PASSWD_ACK(string pass)
    {
      this._pass = pass;
    }

    public override void write()
    {
      this.writeH((short) 3859);
      this.writeS(this._pass, 4);
    }
  }
}
