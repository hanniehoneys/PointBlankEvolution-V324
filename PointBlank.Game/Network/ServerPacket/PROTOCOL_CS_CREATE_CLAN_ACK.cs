using PointBlank.Core.Managers;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_CREATE_CLAN_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Account _p;
    private PointBlank.Core.Models.Account.Clan.Clan clan;
    private uint _erro;

    public PROTOCOL_CS_CREATE_CLAN_ACK(uint erro, PointBlank.Core.Models.Account.Clan.Clan clan, PointBlank.Game.Data.Model.Account player)
    {
      this._erro = erro;
      this.clan = clan;
      this._p = player;
    }

    public override void write()
    {
      this.writeH((short) 1831);
      this.writeD(this._erro);
      if (this._erro != 0U)
        return;
      this.writeD(this.clan._id);
      this.writeUnicode(this.clan._name, 34);
      this.writeC((byte) this.clan._rank);
      this.writeC((byte) PlayerManager.getClanPlayers(this.clan._id));
      this.writeC((byte) this.clan.maxPlayers);
      this.writeD(this.clan.creationDate);
      this.writeD(this.clan._logo);
      this.writeB(new byte[11]);
      this.writeQ(this.clan.owner_id);
      this.writeS(this._p.player_name, 66);
      this.writeC((byte) this._p.name_color);
      this.writeC((byte) this._p._rank);
      this.writeUnicode(this.clan._info, 510);
      this.writeUnicode("Temp", 42);
      this.writeC((byte) this.clan.limite_rank);
      this.writeC((byte) this.clan.limite_idade);
      this.writeC((byte) this.clan.limite_idade2);
      this.writeC((byte) this.clan.autoridade);
      this.writeUnicode("", 510);
      this.writeB(new byte[44]);
      this.writeF((double) this.clan._pontos);
      this.writeF(60.0);
      this.writeB(new byte[16]);
      this.writeF((double) this.clan._pontos);
      this.writeF(60.0);
      this.writeB(new byte[80]);
      this.writeB(new byte[66]);
      this.writeD(this._p._gp);
    }
  }
}
