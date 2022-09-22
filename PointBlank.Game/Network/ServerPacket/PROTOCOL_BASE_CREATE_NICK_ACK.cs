using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_CREATE_NICK_ACK : SendPacket
  {
    private uint _erro;
    private string _name;

    public PROTOCOL_BASE_CREATE_NICK_ACK(uint erro, string name)
    {
      this._erro = erro;
      this._name = name;
    }

    public override void write()
    {
      this.writeH((short) 535);
      this.writeH((short) 0);
      this.writeD(1500511);
      this.writeD(1075242335);
      this.writeC((byte) this._name.Length);
      this.writeUnicode(this._name, this._name.Length * 2);
      this.writeD(this._erro);
    }
  }
}
