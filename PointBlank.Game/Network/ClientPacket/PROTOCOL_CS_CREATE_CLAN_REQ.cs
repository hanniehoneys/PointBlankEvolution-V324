using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_CREATE_CLAN_REQ : ReceivePacket
  {
    private uint erro;
    private string clanName;
    private string clanInfo;

    public PROTOCOL_CS_CREATE_CLAN_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.readD();
      this.clanName = this.readUnicode(34);
      this.clanInfo = this.readUnicode(510);
      this.readD();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        PointBlank.Core.Models.Account.Clan.Clan clan = new PointBlank.Core.Models.Account.Clan.Clan()
        {
          _name = this.clanName,
          _info = this.clanInfo,
          _logo = 0,
          owner_id = player.player_id,
          creationDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"))
        };
        if (player.clanId > 0 || PlayerManager.getRequestClanId(player.player_id) > 0)
          this.erro = 2147487836U;
        else if (0 > player._gp - GameConfig.minCreateGold || GameConfig.minCreateRank > player._rank)
        {
          this.erro = 2147487818U;
        }
        else
        {
          if (ClanManager.isClanNameExist(clan._name))
          {
            this.erro = 2147487834U;
            return;
          }
          if (ClanManager._clans.Count > GameConfig.maxActiveClans)
            this.erro = 2147487829U;
          else if (PlayerManager.CreateClan(out clan._id, clan._name, clan.owner_id, clan._info, clan.creationDate) && PlayerManager.updateAccountGold(player.player_id, player._gp - GameConfig.minCreateGold))
          {
            clan.BestPlayers.SetDefault();
            player.clanDate = clan.creationDate;
            if (ComDiv.updateDB("accounts", "player_id", (object) player.player_id, new string[3]
            {
              "clanaccess",
              "clandate",
              "clan_id"
            }, (object) 1, (object) clan.creationDate, (object) clan._id))
            {
              if (clan._id > 0)
              {
                player.clanId = clan._id;
                player.clanAccess = 1;
                ClanManager.AddClan(clan);
                SendClanInfo.Load(clan, 0);
                player._gp -= GameConfig.minCreateGold;
              }
              else
                this.erro = 2147487819U;
            }
            else
              this.erro = 2147487816U;
          }
          else
            this.erro = 2147487816U;
        }
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_CREATE_CLAN_ACK(this.erro, clan, player));
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_CS_CREATE_CLAN_REQ: " + ex.ToString());
      }
    }
  }
}
