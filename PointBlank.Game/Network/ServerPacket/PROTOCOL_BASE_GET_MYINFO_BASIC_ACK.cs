using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_GET_MYINFO_BASIC_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Account p;
    private PointBlank.Core.Models.Account.Clan.Clan clan;

    public PROTOCOL_BASE_GET_MYINFO_BASIC_ACK(PointBlank.Game.Data.Model.Account player)
    {
      this.p = player;
      this.clan = ClanManager.getClan(this.p.clanId);
    }

    public override void write()
    {
      this.writeH((short) 579);
      this.writeUnicode(this.p.player_name, 66);
      this.writeD(this.p._rank);
      this.writeD(this.p._rank);
      this.writeD(this.p._gp);
      this.writeD(this.p._exp);
      this.writeD(0);
      this.writeC((byte) 0);
      this.writeD(0);
      this.writeQ(0L);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeD(this.p._money);
      this.writeD(this.clan._id);
      this.writeD(this.p.clanAccess);
      this.writeQ(0L);
      this.writeC((byte) this.p.pc_cafe);
      this.writeC((byte) this.p.tourneyLevel);
      this.writeUnicode(this.clan._name, 34);
      this.writeC((byte) this.clan._rank);
      this.writeC((byte) this.clan.getClanUnit());
      this.writeD(this.clan._logo);
      this.writeC((byte) this.clan._name_color);
      this.writeC((byte) this.clan.effect);
      this.writeD(this.p._statistic.fights);
      this.writeD(this.p._statistic.fights_win);
      this.writeD(this.p._statistic.fights_lost);
      this.writeD(this.p._statistic.fights_draw);
      this.writeD(this.p._statistic.kills_count);
      this.writeD(this.p._statistic.headshots_count);
      this.writeD(this.p._statistic.deaths_count);
      this.writeD(this.p._statistic.totalfights_count);
      this.writeD(this.p._statistic.totalkills_count);
      this.writeD(this.p._statistic.escapes);
      this.writeD(this.p._statistic.assist);
      this.writeD(this.p._statistic.fights);
      this.writeD(this.p._statistic.fights_win);
      this.writeD(this.p._statistic.fights_lost);
      this.writeD(this.p._statistic.fights_draw);
      this.writeD(this.p._statistic.kills_count);
      this.writeD(this.p._statistic.headshots_count);
      this.writeD(this.p._statistic.deaths_count);
      this.writeD(this.p._statistic.totalfights_count);
      this.writeD(this.p._statistic.totalkills_count);
      this.writeD(this.p._statistic.escapes);
      this.writeD(this.p._statistic.assist);
    }
  }
}
