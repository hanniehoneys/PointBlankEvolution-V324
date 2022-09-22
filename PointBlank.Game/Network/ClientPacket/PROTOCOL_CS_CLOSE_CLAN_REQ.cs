using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_CLOSE_CLAN_REQ : ReceivePacket
  {
    private uint erro;

    public PROTOCOL_CS_CLOSE_CLAN_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player != null)
        {
          PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
          if (clan._id > 0 && clan.owner_id == this._client.player_id && ComDiv.deleteDB("clan_data", "clan_id", (object) clan._id))
          {
            if (ComDiv.updateDB("accounts", "player_id", (object) player.player_id, new string[4]
            {
              "clan_id",
              "clanaccess",
              "clan_game_pt",
              "clan_wins_pt"
            }, (object) 0, (object) 0, (object) 0, (object) 0) && ClanManager.RemoveClan(clan))
            {
              player.clanId = 0;
              player.clanAccess = 0;
              SendClanInfo.Load(clan, 1);
              goto label_6;
            }
          }
          this.erro = 2147487850U;
        }
        else
          this.erro = 2147487850U;
label_6:
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_CLOSE_CLAN_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_CLOSE_CLAN_REQ: " + ex.ToString());
      }
    }
  }
}
