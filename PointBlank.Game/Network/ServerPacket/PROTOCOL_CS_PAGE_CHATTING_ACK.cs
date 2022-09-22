using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_PAGE_CHATTING_ACK : SendPacket
  {
    private string sender;
    private string message;
    private int type;
    private int bantime;
    private int name_color;
    private bool isGM;

    public PROTOCOL_CS_PAGE_CHATTING_ACK(Account p, string msg)
    {
      this.sender = p.player_name;
      this.message = msg;
      this.isGM = p.UseChatGM();
      this.name_color = p.name_color;
    }

    public PROTOCOL_CS_PAGE_CHATTING_ACK(int type, int bantime)
    {
      this.type = type;
      this.bantime = bantime;
    }

    public override void write()
    {
      this.writeH((short) 1911);
      this.writeC((byte) this.type);
      if (this.type == 0)
      {
        this.writeC((byte) (this.sender.Length + 1));
        this.writeUnicode(this.sender, true);
        this.writeC(this.isGM);
        this.writeC((byte) this.name_color);
        this.writeC((byte) (this.message.Length + 1));
        this.writeUnicode(this.message, true);
      }
      else
        this.writeD(this.bantime);
    }
  }
}
