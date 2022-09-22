using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CHATTING_ACK : SendPacket
  {
    private string text;
    private Account p;
    private int type;
    private int bantime;

    public PROTOCOL_CS_CHATTING_ACK(string text, Account player)
    {
      this.text = text;
      this.p = player;
    }

    public PROTOCOL_CS_CHATTING_ACK(int type, int bantime)
    {
      this.type = type;
      this.bantime = bantime;
    }

    public override void write()
    {
      this.writeH((short) 1879);
      if (this.type == 0)
      {
        this.writeC((byte) (this.p.player_name.Length + 1));
        this.writeUnicode(this.p.player_name, true);
        this.writeC(this.p.UseChatGM());
        this.writeC((byte) this.p.name_color);
        this.writeC((byte) (this.text.Length + 1));
        this.writeUnicode(this.text, true);
      }
      else
        this.writeD(this.bantime);
    }
  }
}
