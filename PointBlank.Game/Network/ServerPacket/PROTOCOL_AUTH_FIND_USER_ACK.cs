using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_FIND_USER_ACK : SendPacket
  {
    private uint _erro;
    private Account player;

    public PROTOCOL_AUTH_FIND_USER_ACK(uint erro, Account player)
    {
      this._erro = erro;
      this.player = player;
    }

    public override void write()
    {
      this.writeH((short) 810);
      this.writeD(this._erro);
      if (this._erro != 0U)
        return;
      this.writeC((byte) this.player.getRank());
      this.writeD(ComDiv.GetPlayerStatus(this.player._status, this.player._isOnline));
      this.writeUnicode(ClanManager.getClan(this.player.clanId)._name, 34);
      this.writeD(this.player._statistic.fights);
      this.writeD(this.player._statistic.fights_win);
      this.writeD(this.player._statistic.fights_lost);
      this.writeD(this.player._statistic.fights_draw);
      this.writeD(this.player._statistic.kills_count);
      this.writeD(this.player._statistic.headshots_count);
      this.writeD(this.player._statistic.deaths_count);
      this.writeD(this.player._statistic.totalfights_count);
      this.writeD(this.player._statistic.totalkills_count);
      this.writeD(this.player._statistic.escapes);
      this.writeD(this.player._statistic.assist);
      this.writeD(this.player._statistic.fights);
      this.writeD(this.player._statistic.fights_win);
      this.writeD(this.player._statistic.fights_lost);
      this.writeD(this.player._statistic.fights_draw);
      this.writeD(this.player._statistic.kills_count);
      this.writeD(this.player._statistic.headshots_count);
      this.writeD(this.player._statistic.deaths_count);
      this.writeD(this.player._statistic.totalfights_count);
      this.writeD(this.player._statistic.totalkills_count);
      this.writeD(this.player._statistic.escapes);
      this.writeD(this.player._statistic.assist);
      this.writeD(this.player._equip._primary);
      this.writeD(this.player._equip._secondary);
      this.writeD(this.player._equip._melee);
      this.writeD(this.player._equip._grenade);
      this.writeD(this.player._equip._special);
      this.writeD(this.player._equip._red);
      this.writeD(this.player._equip.face);
      this.writeD(this.player._equip._helmet);
      this.writeD(this.player._equip.jacket);
      this.writeD(this.player._equip.poket);
      this.writeD(this.player._equip.glove);
      this.writeD(this.player._equip.belt);
      this.writeD(this.player._equip.holster);
      this.writeD(this.player._equip.skin);
      this.writeD(this.player._equip._beret);
      this.writeD(this.player._equip._red);
      this.writeD(this.player._equip._blue);
    }
  }
}
