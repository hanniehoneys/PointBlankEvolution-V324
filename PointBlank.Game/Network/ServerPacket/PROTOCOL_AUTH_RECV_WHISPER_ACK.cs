using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_RECV_WHISPER_ACK : SendPacket
  {
    private string _sender;
    private string _msg;
    private bool chatGM;

    public PROTOCOL_AUTH_RECV_WHISPER_ACK(string sender, string msg, bool chatGM)
    {
      this._sender = sender;
      this._msg = msg;
      this.chatGM = chatGM;
    }

    public override void write()
    {
      this.writeH((short) 806);
      this.writeUnicode(this._sender, 66);
      this.writeC(this.chatGM);
      this.writeC((byte) 0);
      this.writeH((ushort) (this._msg.Length + 1));
      this.writeUnicode(this._msg, true);
    }
  }
}
