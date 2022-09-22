using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_MEDAL_GET_INFO_ACK : SendPacket
  {
    private Account p;

    public PROTOCOL_BASE_MEDAL_GET_INFO_ACK(Account p)
    {
      this.p = p;
    }

    public override void write()
    {
      this.writeH((short) 571);
      if (this.p != null)
      {
        this.writeQ(this.p.player_id);
        this.writeD(this.p.brooch);
        this.writeD(this.p.insignia);
        this.writeD(this.p.medal);
        this.writeD(this.p.blue_order);
      }
      else
        this.writeB(new byte[24]);
    }
  }
}
