using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_SECESSION_CLAN_REQ : ReceivePacket
  {
    private uint erro;

    public PROTOCOL_CS_SECESSION_CLAN_REQ(GameClient client, byte[] data)
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
        if (this._client == null)
          return;
        PointBlank.Game.Data.Model.Account player = this._client._player;
        try
        {
          if (player != null && player.clanId > 0)
          {
            PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
            if (clan._id > 0 && clan.owner_id != player.player_id)
            {
              if (ComDiv.updateDB("accounts", "player_id", (object) player.player_id, new string[4]
              {
                "clan_id",
                "clanaccess",
                "clan_game_pt",
                "clan_wins_pt"
              }, (object) 0, (object) 0, (object) 0, (object) 0))
              {
                using (PROTOCOL_CS_MEMBER_INFO_DELETE_ACK memberInfoDeleteAck = new PROTOCOL_CS_MEMBER_INFO_DELETE_ACK(player.player_id))
                  ClanManager.SendPacket((SendPacket) memberInfoDeleteAck, player.clanId, player.player_id, true, true);
                long ownerId = clan.owner_id;
                if (MessageManager.getMsgsCount(ownerId) < 100)
                {
                  Message message = this.CreateMessage(clan, player);
                  if (message != null)
                  {
                    PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(ownerId, 0);
                    if (account != null && account._isOnline)
                      account.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(message), false);
                  }
                }
                player.clanId = 0;
                player.clanAccess = 0;
              }
              else
                this.erro = 2147487851U;
            }
            else
              this.erro = 2147487838U;
          }
          else
            this.erro = 2147487835U;
        }
        catch
        {
          this.erro = 2147487851U;
        }
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_SECESSION_CLAN_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_SECESSION_CLAN_REQ: " + ex.ToString());
      }
    }

    private Message CreateMessage(PointBlank.Core.Models.Account.Clan.Clan clan, PointBlank.Game.Data.Model.Account sender)
    {
      Message msg = new Message(15.0)
      {
        sender_name = clan._name,
        sender_id = sender.player_id,
        clanId = clan._id,
        type = 4,
        text = sender.player_name,
        state = 1,
        cB = NoteMessageClan.Secession
      };
      if (!MessageManager.CreateMessage(clan.owner_id, msg))
        return (Message) null;
      return msg;
    }
  }
}
