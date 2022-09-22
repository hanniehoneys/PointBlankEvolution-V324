using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_USER_TITLE_CHANGE_ACK : SendPacket
  {
    private int _slots;
    private uint _erro;

    public PROTOCOL_BASE_USER_TITLE_CHANGE_ACK(uint erro, int slots)
    {
      this._erro = erro;
      this._slots = slots;
    }

    public override void write()
    {
      this.writeH((short) 585);
      this.writeD(this._erro);
      this.writeD(this._slots);
    }
  }
}
