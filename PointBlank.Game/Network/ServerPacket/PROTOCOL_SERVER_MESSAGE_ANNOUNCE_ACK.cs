using PointBlank.Core;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK : SendPacket
  {
    private string _message;

    public PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(string msg)
    {
      this._message = msg;
      if (msg.Length < 1024)
        return;
      Logger.error("Message with size bigger than 1024 sent!!");
    }

    public override void write()
    {
      this.writeH((short) 2567);
      this.writeH((short) 0);
      this.writeD(0);
      this.writeH((ushort) this._message.Length);
      this.writeUnicode(this._message, false);
      this.writeD(2);
    }
  }
}
