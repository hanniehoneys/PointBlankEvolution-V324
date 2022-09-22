using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_REPLACE_NAME_RESULT_ACK : SendPacket
  {
    private string _name;

    public PROTOCOL_CS_REPLACE_NAME_RESULT_ACK(string name)
    {
      this._name = name;
    }

    public override void write()
    {
      this.writeH((short) 1888);
      this.writeUnicode(this._name, 34);
    }
  }
}
