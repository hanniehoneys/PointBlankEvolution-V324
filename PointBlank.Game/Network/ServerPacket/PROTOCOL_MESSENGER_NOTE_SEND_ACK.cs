using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_MESSENGER_NOTE_SEND_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_MESSENGER_NOTE_SEND_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 930);
      this.writeD(this._erro);
    }
  }
}
