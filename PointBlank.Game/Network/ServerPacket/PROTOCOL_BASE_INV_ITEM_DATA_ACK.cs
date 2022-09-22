using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_INV_ITEM_DATA_ACK : SendPacket
  {
    private int Type;
    private PlayerBonus Bonus;
    private PointBlank.Game.Data.Model.Account Player;

    public PROTOCOL_BASE_INV_ITEM_DATA_ACK(int Type, PointBlank.Game.Data.Model.Account Player)
    {
      this.Type = Type;
      this.Player = Player;
      this.Bonus = Player._bonus;
    }

    public override void write()
    {
      this.writeH((short) 603);
      this.writeC((byte) this.Type);
      this.writeC((byte) this.Player.name_color);
      this.writeD(this.Bonus.fakeRank);
      this.writeD(this.Bonus.fakeRank);
      this.writeUnicode(this.Bonus.fakeNick, 66);
      this.writeH((short) this.Bonus.sightColor);
      this.writeH((short) this.Bonus.muzzle);
    }
  }
}
