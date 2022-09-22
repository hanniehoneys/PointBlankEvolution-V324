using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CHAR_CHANGE_EQUIP_ACK : SendPacket
  {
    private uint Error;

    public PROTOCOL_CHAR_CHANGE_EQUIP_ACK(uint Error)
    {
      this.Error = Error;
    }

    public override void write()
    {
      this.writeH((short) 6150);
      this.writeH((short) 0);
      this.writeD(this.Error);
    }
  }
}
