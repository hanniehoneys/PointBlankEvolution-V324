using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_DAILY_RECORD_ACK : SendPacket
  {
    private PlayerDailyRecord Record;

    public PROTOCOL_BASE_DAILY_RECORD_ACK(PlayerDailyRecord Record)
    {
      this.Record = Record;
    }

    public override void write()
    {
      this.writeH((short) 623);
      this.writeH((short) this.Record.Total);
      this.writeH((short) this.Record.Wins);
      this.writeH((short) this.Record.Loses);
      this.writeH((short) this.Record.Draws);
      this.writeH((short) this.Record.Kills);
      this.writeH((short) this.Record.Headshots);
      this.writeH((short) this.Record.Deaths);
      this.writeD(this.Record.Exp);
      this.writeD(this.Record.Point);
      this.writeD(0);
      this.writeC((byte) 0);
      this.writeD(0);
      this.writeH((short) 1);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeC((byte) 0);
    }
  }
}
