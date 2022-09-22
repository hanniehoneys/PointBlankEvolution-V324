using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_REPLACE_MANAGEMENT_REQ : ReceivePacket
  {
    private int limite_rank;
    private int limite_idade;
    private int limite_idade2;
    private int autoridade;
    private uint erro;

    public PROTOCOL_CS_REPLACE_MANAGEMENT_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.autoridade = (int) this.readC();
      this.limite_rank = (int) this.readC();
      this.limite_idade = (int) this.readC();
      this.limite_idade2 = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
        if (clan._id > 0 && clan.owner_id == this._client.player_id && PlayerManager.updateClanInfo(clan._id, this.autoridade, this.limite_rank, this.limite_idade, this.limite_idade2))
        {
          clan.autoridade = this.autoridade;
          clan.limite_rank = this.limite_rank;
          clan.limite_idade = this.limite_idade;
          clan.limite_idade2 = this.limite_idade2;
        }
        else
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_REPLACE_MANAGEMENT_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
