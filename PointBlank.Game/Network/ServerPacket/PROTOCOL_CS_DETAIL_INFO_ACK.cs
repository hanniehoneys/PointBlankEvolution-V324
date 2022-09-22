using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_DETAIL_INFO_ACK : SendPacket
  {
    private PointBlank.Core.Models.Account.Clan.Clan clan;
    private int _erro;

    public PROTOCOL_CS_DETAIL_INFO_ACK(int erro, PointBlank.Core.Models.Account.Clan.Clan c)
    {
      this._erro = erro;
      this.clan = c;
    }

    public override void write()
    {
      PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(this.clan.owner_id, 0);
      int clanPlayers = PlayerManager.getClanPlayers(this.clan._id);
      this.writeH((short) 1825);
      this.writeD(this._erro);
      this.writeD(this.clan._id);
      this.writeUnicode(this.clan._name, 34);
      this.writeC((byte) this.clan._rank);
      this.writeC((byte) clanPlayers);
      this.writeC((byte) this.clan.maxPlayers);
      this.writeD(this.clan.creationDate);
      this.writeD(this.clan._logo);
      this.writeC((byte) this.clan._name_color);
      this.writeC((byte) this.clan.effect);
      this.writeC((byte) this.clan.getClanUnit());
      this.writeD(this.clan._exp);
      this.writeD(10);
      this.writeQ(this.clan.owner_id);
      this.writeUnicode(account != null ? account.player_name : "", 66);
      this.writeC(account != null ? (byte) account.name_color : (byte) 0);
      this.writeC(account != null ? (byte) account._rank : (byte) 0);
      this.writeUnicode(this.clan._info, 510);
      this.writeUnicode("Temp", 42);
      this.writeC((byte) this.clan.limite_rank);
      this.writeC((byte) this.clan.limite_idade);
      this.writeC((byte) this.clan.limite_idade2);
      this.writeC((byte) this.clan.autoridade);
      this.writeUnicode(this.clan._news, 510);
      this.writeD(this.clan.partidas);
      this.writeD(this.clan.vitorias);
      this.writeD(this.clan.derrotas);
      this.writeD(this.clan.partidas);
      this.writeD(this.clan.vitorias);
      this.writeD(this.clan.derrotas);
      this.writeD(0);
      this.writeD(this.clan.partidas);
      this.writeD(this.clan.vitorias);
      this.writeD(this.clan.derrotas);
      this.writeD(0);
      this.writeF((double) this.clan._pontos);
      this.writeF(60.0);
      this.writeD(this.clan.partidas);
      this.writeD(this.clan.vitorias);
      this.writeD(this.clan.derrotas);
      this.writeD(0);
      this.writeF((double) this.clan._pontos);
      this.writeF(60.0);
      this.writeQ(this.clan.BestPlayers.Exp.PlayerId);
      this.writeQ(this.clan.BestPlayers.Exp.PlayerId);
      this.writeQ(this.clan.BestPlayers.Wins.PlayerId);
      this.writeQ(this.clan.BestPlayers.Wins.PlayerId);
      this.writeQ(this.clan.BestPlayers.Kills.PlayerId);
      this.writeQ(this.clan.BestPlayers.Kills.PlayerId);
      this.writeQ(this.clan.BestPlayers.Headshot.PlayerId);
      this.writeQ(this.clan.BestPlayers.Headshot.PlayerId);
      this.writeQ(this.clan.BestPlayers.Participation.PlayerId);
      this.writeQ(this.clan.BestPlayers.Participation.PlayerId);
      this.writeB(new byte[66]);
    }
  }
}
