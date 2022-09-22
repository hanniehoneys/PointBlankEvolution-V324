using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_CHANGE_NICKNAME_ACK : SendPacket
  {
    private string name;

    public PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(string name)
    {
      this.name = name;
    }

    public override void write()
    {
      this.writeH((short) 812);
      this.writeC((byte) this.name.Length);
      this.writeUnicode(this.name, this.name.Length * 2);
    }
  }
}
