using PointBlank.Core.Network;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SHOP_ENTER_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 1026);
      this.writeC((byte) 0);
      this.writeD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
    }
  }
}
