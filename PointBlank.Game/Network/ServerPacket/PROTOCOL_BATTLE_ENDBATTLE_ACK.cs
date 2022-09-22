using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Utils;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_ENDBATTLE_ACK : SendPacket
  {
    private int Winner = 2;
    private PointBlank.Game.Data.Model.Room r;
    private PointBlank.Game.Data.Model.Account p;
    private ushort PlayersFlag;
    private ushort MissionsFlag;
    private bool BotMode;

    public PROTOCOL_BATTLE_ENDBATTLE_ACK(PointBlank.Game.Data.Model.Account p)
    {
      this.p = p;
      if (p == null)
        return;
      this.r = p._room;
      this.Winner = this.r.room_type != RoomType.Tutorial ? (int) AllUtils.GetWinnerTeam(this.r) : 0;
      this.BotMode = this.r.isBotMode();
      AllUtils.getBattleResult(this.r, out this.MissionsFlag, out this.PlayersFlag);
    }

    public PROTOCOL_BATTLE_ENDBATTLE_ACK(
      PointBlank.Game.Data.Model.Account p,
      int Winner,
      ushort PlayersFlag,
      ushort MissionsFlag,
      bool BotMode)
    {
      this.p = p;
      this.Winner = Winner;
      this.PlayersFlag = PlayersFlag;
      this.MissionsFlag = MissionsFlag;
      this.BotMode = BotMode;
      if (p == null)
        return;
      this.r = p._room;
    }

    public PROTOCOL_BATTLE_ENDBATTLE_ACK(
      PointBlank.Game.Data.Model.Account p,
      TeamResultType Winner,
      ushort PlayersFlag,
      ushort MissionsFlag,
      bool BotMode)
    {
      this.p = p;
      this.Winner = (int) Winner;
      this.PlayersFlag = PlayersFlag;
      this.MissionsFlag = MissionsFlag;
      this.BotMode = BotMode;
      if (p == null)
        return;
      this.r = p._room;
    }

    public override void write()
    {
      if (this.p == null || this.r == null)
        return;
      PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(this.p.clanId);
      this.writeH((short) 4116);
      this.writeH(this.PlayersFlag);
      this.writeC((byte) this.Winner);
      for (int index = 0; index < 16; ++index)
        this.writeH((ushort) this.r._slots[index].exp);
      for (int index = 0; index < 16; ++index)
        this.writeH((ushort) this.r._slots[index].gp);
      for (int index = 0; index < 16; ++index)
        this.writeC((byte) this.r._slots[index].bonusFlags);
      for (int index = 0; index < 16; ++index)
      {
        Slot slot = this.r._slots[index];
        this.writeH((ushort) slot.BonusCafeExp);
        this.writeH((ushort) slot.BonusItemExp);
        this.writeH((ushort) slot.BonusEventExp);
      }
      for (int index = 0; index < 16; ++index)
      {
        Slot slot = this.r._slots[index];
        this.writeH((ushort) slot.BonusCafePoint);
        this.writeH((ushort) slot.BonusItemPoint);
        this.writeH((ushort) slot.BonusEventPoint);
      }
      this.writeH(this.MissionsFlag);
      if (this.BotMode)
      {
        for (int index = 0; index < 16; ++index)
          this.writeH((ushort) this.r._slots[index].Score);
      }
      else if (this.r.room_type == RoomType.Bomb || this.r.room_type == RoomType.Annihilation || (this.r.room_type == RoomType.Boss || this.r.room_type == RoomType.CrossCounter) || (this.r.room_type == RoomType.Convoy || this.r.room_type == RoomType.Defense || this.r.room_type == RoomType.Destroy))
      {
        this.writeH(this.r.room_type == RoomType.Boss ? (ushort) this.r.red_dino : (this.r.room_type == RoomType.CrossCounter ? (ushort) this.r._redKills : (ushort) this.r.red_rounds));
        this.writeH(this.r.room_type == RoomType.Boss ? (ushort) this.r.blue_dino : (this.r.room_type == RoomType.CrossCounter ? (ushort) this.r._blueKills : (ushort) this.r.blue_rounds));
        for (int index = 0; index < 16; ++index)
          this.writeC((byte) this.r._slots[index].objects);
      }
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeB(new byte[25]
      {
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue,
        byte.MaxValue
      });
      this.writeC((byte) (this.p.player_name.Length * 2));
      this.writeUnicode(this.p.player_name, this.p.player_name.Length * 2);
      this.writeD(this.p.getRank());
      this.writeD(this.p.getRank());
      this.writeD(this.p._gp);
      this.writeD(this.p._exp);
      this.writeD(0);
      this.writeC((byte) 0);
      this.writeD(0);
      this.writeQ(0L);
      this.writeC((byte) 0);
      this.writeC((byte) 0);
      this.writeD(this.p._money);
      this.writeD(clan._id);
      this.writeD(this.p.clanAccess);
      this.writeQ(0L);
      this.writeC((byte) this.p.pc_cafe);
      this.writeC((byte) this.p.tourneyLevel);
      this.writeC((byte) (clan._name.Length * 2));
      this.writeUnicode(clan._name, clan._name.Length * 2);
      this.writeC((byte) clan._rank);
      this.writeC((byte) clan.getClanUnit());
      this.writeD(clan._logo);
      this.writeC((byte) clan._name_color);
      this.writeC((byte) clan.effect);
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
      this.writeH((short) this.p.Daily.Total);
      this.writeH((short) this.p.Daily.Wins);
      this.writeH((short) this.p.Daily.Loses);
      this.writeH((short) this.p.Daily.Draws);
      this.writeH((short) this.p.Daily.Kills);
      this.writeH((short) this.p.Daily.Headshots);
      this.writeH((short) this.p.Daily.Deaths);
      this.writeD(this.p.Daily.Exp);
      this.writeD(this.p.Daily.Point);
      this.writeB(new byte[16]);
      this.writeC((byte) 0);
      this.writeD(0);
      this.writeD(0);
    }
  }
}
