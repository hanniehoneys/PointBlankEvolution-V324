using PointBlank.Core.Models.Account;
using PointBlank.Core.Network;
using System;
using System.Collections.Generic;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_USER_GIFTLIST_ACK : SendPacket
  {
    private int erro;
    private List<Message> gifts;

    public PROTOCOL_BASE_USER_GIFTLIST_ACK(int erro, List<Message> gifts)
    {
      this.erro = erro;
      this.gifts = gifts;
    }

    public override void write()
    {
      this.writeH((short) 684);
      this.writeH((short) 0);
      this.writeC((byte) this.gifts.Count);
      for (int index = 0; index < this.gifts.Count; ++index)
      {
        Message gift = this.gifts[index];
      }
      this.writeD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
    }
  }
}
