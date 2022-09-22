using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_ACCEPT_REQUEST_RESULT_ACK : SendPacket
  {
    private PointBlank.Core.Models.Account.Clan.Clan clan;
    private PointBlank.Game.Data.Model.Account p;
    private int players;

    public PROTOCOL_CS_ACCEPT_REQUEST_RESULT_ACK(PointBlank.Core.Models.Account.Clan.Clan c, PointBlank.Game.Data.Model.Account owner, int clanPlayers)
    {
      this.clan = c;
      this.p = owner;
      this.players = clanPlayers;
    }

    public PROTOCOL_CS_ACCEPT_REQUEST_RESULT_ACK(PointBlank.Core.Models.Account.Clan.Clan c, int clanPlayers)
    {
      this.clan = c;
      this.p = AccountManager.getAccount(this.clan.owner_id, 0);
      this.players = clanPlayers;
    }

    public override void write()
    {
      this.writeH((short) 1848);
      this.writeD(this.clan._id);
      this.writeUnicode(this.clan._name, 34);
      this.writeC((byte) this.clan._rank);
      this.writeC((byte) this.players);
      this.writeC((byte) this.clan.maxPlayers);
      this.writeD(this.clan.creationDate);
      this.writeD(this.clan._logo);
      this.writeC((byte) this.clan._name_color);
      this.writeC((byte) this.clan.effect);
      this.writeC((byte) this.clan.getClanUnit());
      this.writeD(this.clan._exp);
      this.writeD(10);
      this.writeQ(this.clan.owner_id);
      this.writeUnicode(this.p != null ? this.p.player_name : "", 66);
      this.writeC(this.p != null ? (byte) this.p.name_color : (byte) 0);
      this.writeC(this.p != null ? (byte) this.p._rank : (byte) 0);
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
