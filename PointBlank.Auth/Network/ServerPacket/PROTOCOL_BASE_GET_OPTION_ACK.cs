using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_GET_OPTION_ACK : SendPacket
  {
    private int error;
    private PlayerConfig c;
    private bool isValid;

    public PROTOCOL_BASE_GET_OPTION_ACK(int error, PlayerConfig config)
    {
      this.error = error;
      this.c = config;
      this.isValid = this.c != null;
    }

    public PROTOCOL_BASE_GET_OPTION_ACK(int error)
    {
      this.error = error;
    }

    public override void write()
    {
      this.writeH((short) 529);
      this.writeH((short) 0);
      this.writeD(this.error);
      if (this.error < 0)
        return;
      if (this.isValid)
      {
        this.writeH((ushort) this.c.macro_5.Length);
        this.writeUnicode(this.c.macro_5, false);
        this.writeH((ushort) this.c.macro_4.Length);
        this.writeUnicode(this.c.macro_4, false);
        this.writeH((ushort) this.c.macro_3.Length);
        this.writeUnicode(this.c.macro_3, false);
        this.writeH((ushort) this.c.macro_2.Length);
        this.writeUnicode(this.c.macro_2, false);
        this.writeH((ushort) this.c.macro_1.Length);
        this.writeUnicode(this.c.macro_1, false);
        this.writeH((short) 48);
        this.writeB(new byte[4]
        {
          (byte) 57,
          (byte) 248,
          (byte) 16,
          (byte) 0
        });
        this.writeB(this.c.keys);
        this.writeH((short) this.c.blood);
        this.writeC((byte) this.c.sight);
        this.writeC((byte) this.c.hand);
        this.writeD(this.c.config);
        this.writeD(this.c.audio_enable);
        this.writeH((short) 0);
        this.writeC((byte) this.c.audio1);
        this.writeC((byte) this.c.audio2);
        this.writeC((byte) this.c.fov);
        this.writeC((byte) 0);
        this.writeC((byte) this.c.sensibilidade);
        this.writeC((byte) this.c.mouse_invertido);
        this.writeH((short) 0);
        this.writeC((byte) this.c.msgConvite);
        this.writeC((byte) this.c.chatSussurro);
        this.writeD(this.c.macro);
      }
      else
      {
        this.writeB(new byte[39]);
        this.writeC(!this.isValid);
      }
    }
  }
}
