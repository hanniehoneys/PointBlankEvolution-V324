using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SHOP_CAPSULE_ACK : SendPacket
  {
    private List<ItemsModel> Rewards;
    private int CouponId;
    private int Index;

    public PROTOCOL_AUTH_SHOP_CAPSULE_ACK(List<ItemsModel> Rewards, int CouponId, int Index)
    {
      this.CouponId = CouponId;
      this.Index = Index;
      this.Rewards = Rewards;
    }

    public override void write()
    {
      this.writeH((short) 1064);
      this.writeH((short) 0);
      this.writeC((byte) this.Rewards.Count);
      for (int index = 0; index < this.Rewards.Count; ++index)
      {
        ItemsModel reward = this.Rewards[index];
        this.writeD(reward._id);
        this.writeD((int) reward._count);
      }
      this.writeC((byte) this.Index);
      this.writeD(this.CouponId);
    }
  }
}
