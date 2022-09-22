using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_LOBBY_CHATTING_ACK : SendPacket
  {
    private string sender;
    private string msg;
    private uint sessionId;
    private int nameColor;
    private bool GMColor;

    public PROTOCOL_LOBBY_CHATTING_ACK(Account player, string message, bool GMCmd = false)
    {
      if (!GMCmd)
      {
        this.nameColor = player.name_color;
        this.GMColor = player.UseChatGM();
      }
      else
        this.GMColor = true;
      this.sender = player.player_name;
      this.sessionId = player.getSessionId();
      this.msg = message;
    }

    public PROTOCOL_LOBBY_CHATTING_ACK(
      string snd,
      uint session,
      int name_color,
      bool chatGm,
      string message)
    {
      this.sender = snd;
      this.sessionId = session;
      this.nameColor = name_color;
      this.GMColor = chatGm;
      this.msg = message;
    }

    public override void write()
    {
      this.writeH((short) 3087);
      this.writeD(this.sessionId);
      this.writeC((byte) (this.sender.Length + 1));
      this.writeUnicode(this.sender, true);
      this.writeC((byte) this.nameColor);
      this.writeC(this.GMColor);
      this.writeH((ushort) (this.msg.Length + 1));
      this.writeUnicode(this.msg, true);
    }
  }
}
