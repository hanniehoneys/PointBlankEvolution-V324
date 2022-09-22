using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SEND_WHISPER_ACK : SendPacket
  {
    private string name;
    private string msg;
    private uint erro;
    private int type;
    private int bantime;

    public PROTOCOL_AUTH_SEND_WHISPER_ACK(string name, string msg, uint erro)
    {
      this.name = name;
      this.msg = msg;
      this.erro = erro;
    }

    public PROTOCOL_AUTH_SEND_WHISPER_ACK(int type, int bantime)
    {
      this.type = type;
      this.bantime = bantime;
    }

    public override void write()
    {
      this.writeH((short) 805);
      this.writeC((byte) this.type);
      if (this.type == 0)
      {
        this.writeD(this.erro);
        this.writeUnicode(this.name, 66);
        if (this.erro != 0U)
          return;
        this.writeH((ushort) (this.msg.Length + 1));
        this.writeUnicode(this.msg, true);
      }
      else
        this.writeD(this.bantime);
    }
  }
}
