using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_GET_PLAYERINFO_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Account p;

    public PROTOCOL_ROOM_GET_PLAYERINFO_ACK(PointBlank.Game.Data.Model.Account player)
    {
      this.p = player;
    }

    public override void write()
    {
      this.writeH((short) 3842);
      if (this.p == null || this.p._slotId == -1)
      {
        this.writeD(2147483648U);
      }
      else
      {
        PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(this.p.clanId);
        this.writeD(this.p._slotId);
        this.writeS(this.p.player_name, 33);
        this.writeD(this.p._exp);
        this.writeD(this.p.getRank());
        this.writeD(this.p._rank);
        this.writeD(this.p._gp);
        this.writeD(this.p._money);
        this.writeD(clan._id);
        this.writeD(this.p.clanAccess);
        this.writeD(0);
        this.writeD(0);
        this.writeC((byte) this.p.pc_cafe);
        this.writeC((byte) this.p.tourneyLevel);
        this.writeC((byte) this.p.name_color);
        this.writeS(clan._name, 17);
        this.writeC((byte) clan._rank);
        this.writeC((byte) clan.getClanUnit());
        this.writeD(clan._logo);
        this.writeC((byte) clan._name_color);
        this.writeC((byte) 0);
        this.writeD(0);
        this.writeD(0);
        this.writeD(this.p.LastRankUpDate);
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
        this.writeD(this.p._equip._red);
        this.writeD(this.p._equip._blue);
        this.writeD(this.p._equip._helmet);
        this.writeD(this.p._equip._beret);
        this.writeD(this.p._equip._dino);
        this.writeD(this.p._equip._primary);
        this.writeD(this.p._equip._secondary);
        this.writeD(this.p._equip._melee);
        this.writeD(this.p._equip._grenade);
        this.writeD(this.p._equip._special);
        this.writeD(this.p._titles.Equiped1);
        this.writeD(this.p._titles.Equiped2);
        this.writeD(this.p._titles.Equiped3);
      }
    }
  }
}
