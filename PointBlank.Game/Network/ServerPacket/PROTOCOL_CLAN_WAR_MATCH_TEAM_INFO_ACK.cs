using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CLAN_WAR_MATCH_TEAM_INFO_ACK : SendPacket
  {
    private uint _erro;
    private PointBlank.Core.Models.Account.Clan.Clan c;
    private PointBlank.Game.Data.Model.Account leader;

    public PROTOCOL_CLAN_WAR_MATCH_TEAM_INFO_ACK(uint erro, PointBlank.Core.Models.Account.Clan.Clan c)
    {
      this._erro = erro;
      this.c = c;
      if (this.c == null)
        return;
      this.leader = AccountManager.getAccount(this.c.owner_id, 0);
      if (this.leader != null)
        return;
      this._erro = 2147483648U;
    }

    public PROTOCOL_CLAN_WAR_MATCH_TEAM_INFO_ACK(uint erro)
    {
      this._erro = erro;
    }

    public override void write()
    {
      this.writeH((short) 1570);
      this.writeD(this._erro);
      if (this._erro != 0U)
        return;
      int clanPlayers = PlayerManager.getClanPlayers(this.c._id);
      this.writeD(this.c._id);
      this.writeS(this.c._name, 17);
      this.writeC((byte) this.c._rank);
      this.writeC((byte) clanPlayers);
      this.writeC((byte) this.c.maxPlayers);
      this.writeD(this.c.creationDate);
      this.writeD(this.c._logo);
      this.writeC((byte) this.c._name_color);
      this.writeC((byte) this.c.getClanUnit(clanPlayers));
      this.writeD(this.c._exp);
      this.writeD(0);
      this.writeQ(this.c.owner_id);
      this.writeS(this.leader.player_name, 33);
      this.writeC((byte) this.leader._rank);
      this.writeS("", (int) byte.MaxValue);
    }
  }
}
