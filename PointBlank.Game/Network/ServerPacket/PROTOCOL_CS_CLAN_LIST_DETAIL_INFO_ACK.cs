using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CLAN_LIST_DETAIL_INFO_ACK : SendPacket
  {
    private PointBlank.Core.Models.Account.Clan.Clan Clan;
    private int Error;

    public PROTOCOL_CS_CLAN_LIST_DETAIL_INFO_ACK(int Error, PointBlank.Core.Models.Account.Clan.Clan Clan)
    {
      this.Error = Error;
      this.Clan = Clan;
    }

    public override void write()
    {
      PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(this.Clan.owner_id, 0);
      int clanPlayers = PlayerManager.getClanPlayers(this.Clan._id);
      this.writeH((short) 1947);
      this.writeD(this.Error);
      this.writeD(this.Clan._id);
      this.writeUnicode(this.Clan._name, 34);
      this.writeC((byte) this.Clan._rank);
      this.writeC((byte) clanPlayers);
      this.writeC((byte) this.Clan.maxPlayers);
      this.writeD(this.Clan.creationDate);
      this.writeD(this.Clan._logo);
      this.writeC((byte) this.Clan._name_color);
      this.writeC((byte) this.Clan.effect);
      this.writeC((byte) this.Clan.getClanUnit());
      this.writeD(this.Clan._exp);
      this.writeD(10);
      this.writeQ(this.Clan.owner_id);
      this.writeUnicode(account != null ? account.player_name : "", 66);
      this.writeC(account != null ? (byte) account.name_color : (byte) 0);
      this.writeC(account != null ? (byte) account._rank : (byte) 0);
      this.writeUnicode(this.Clan._info, 510);
      this.writeUnicode("Temp", 42);
      this.writeC((byte) this.Clan.limite_rank);
      this.writeC((byte) this.Clan.limite_idade);
      this.writeC((byte) this.Clan.limite_idade2);
      this.writeC((byte) this.Clan.autoridade);
      this.writeUnicode(this.Clan._news, 510);
      this.writeD(this.Clan.partidas);
      this.writeD(this.Clan.vitorias);
      this.writeD(this.Clan.derrotas);
      this.writeD(this.Clan.partidas);
      this.writeD(this.Clan.vitorias);
      this.writeD(this.Clan.derrotas);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeF((double) this.Clan._pontos);
      this.writeF(0.0);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeD(0);
      this.writeF((double) this.Clan._pontos);
      this.writeF(0.0);
      this.writeQ(this.Clan.BestPlayers.Exp.PlayerId);
      this.writeQ(this.Clan.BestPlayers.Exp.PlayerId);
      this.writeQ(this.Clan.BestPlayers.Wins.PlayerId);
      this.writeQ(this.Clan.BestPlayers.Wins.PlayerId);
      this.writeQ(this.Clan.BestPlayers.Kills.PlayerId);
      this.writeQ(this.Clan.BestPlayers.Kills.PlayerId);
      this.writeQ(this.Clan.BestPlayers.Headshot.PlayerId);
      this.writeQ(this.Clan.BestPlayers.Headshot.PlayerId);
      this.writeQ(this.Clan.BestPlayers.Participation.PlayerId);
      this.writeQ(this.Clan.BestPlayers.Participation.PlayerId);
      this.writeB(new byte[66]);
    }
  }
}
