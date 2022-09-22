using PointBlank.Core.Network;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SHOP_GET_SAILLIST_ACK : SendPacket
  {
    private bool Enable;

    public PROTOCOL_SHOP_GET_SAILLIST_ACK(bool Enable)
    {
      this.Enable = Enable;
    }

    public override void write()
    {
      this.writeH((short) 1030);
      this.writeC(this.Enable);
      this.writeD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
    }
  }
}
