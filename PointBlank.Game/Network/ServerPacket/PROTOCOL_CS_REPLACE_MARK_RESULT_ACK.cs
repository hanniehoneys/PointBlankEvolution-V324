using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_REPLACE_MARK_RESULT_ACK : SendPacket
  {
    private uint _logo;

    public PROTOCOL_CS_REPLACE_MARK_RESULT_ACK(uint logo)
    {
      this._logo = logo;
    }

    public override void write()
    {
      this.writeH((short) 1891);
      this.writeD(this._logo);
    }
  }
}
