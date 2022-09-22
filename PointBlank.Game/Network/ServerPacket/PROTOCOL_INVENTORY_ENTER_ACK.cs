using PointBlank.Core.Network;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_INVENTORY_ENTER_ACK : SendPacket
  {
    public override void write()
    {
      this.writeH((short) 3330);
      this.writeD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
    }
  }
}
