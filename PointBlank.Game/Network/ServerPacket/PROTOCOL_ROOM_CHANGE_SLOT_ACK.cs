using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_CHANGE_SLOT_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_ROOM_CHANGE_SLOT_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 3861);
      this.writeD(this._erro);
    }
  }
}
