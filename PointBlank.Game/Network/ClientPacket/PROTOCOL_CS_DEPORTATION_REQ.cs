using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_DEPORTATION_REQ : ReceivePacket
  {
    private uint result;

    public PROTOCOL_CS_DEPORTATION_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      PointBlank.Game.Data.Model.Account player = this._client._player;
      if (player == null)
        return;
      PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
      if (clan._id == 0 || (player.clanAccess < 1 || player.clanAccess > 2) && clan.owner_id != this._client.player_id)
      {
        this.result = 2147487833U;
      }
      else
      {
        List<PointBlank.Game.Data.Model.Account> clanPlayers = ClanManager.getClanPlayers(clan._id, -1L, true);
        int num = (int) this.readC();
        for (int index = 0; index < num; ++index)
        {
          PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(this.readQ(), 0);
          if (account != null && account.clanId == clan._id && account._match == null)
          {
            if (ComDiv.updateDB("accounts", "player_id", (object) account.player_id, new string[4]
            {
              "clan_id",
              "clanaccess",
              "clan_game_pt",
              "clan_wins_pt"
            }, (object) 0, (object) 0, (object) 0, (object) 0))
            {
              using (PROTOCOL_CS_MEMBER_INFO_DELETE_ACK memberInfoDeleteAck = new PROTOCOL_CS_MEMBER_INFO_DELETE_ACK(account.player_id))
                ClanManager.SendPacket((SendPacket) memberInfoDeleteAck, clanPlayers, account.player_id);
              account.clanId = 0;
              account.clanAccess = 0;
              SendClanInfo.Load(account, (PointBlank.Game.Data.Model.Account) null, 0);
              if (MessageManager.getMsgsCount(account.player_id) < 100)
              {
                Message message = this.CreateMessage(clan, account.player_id, this._client.player_id);
                if (message != null && account._isOnline)
                  account.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(message), false);
              }
              if (account._isOnline)
                account.SendPacket((SendPacket) new PROTOCOL_CS_DEPORTATION_RESULT_ACK(), false);
              ++this.result;
              clanPlayers.Remove(account);
              continue;
            }
          }
          this.result = 2147487833U;
          break;
        }
      }
    }

    public override void run()
    {
      try
      {
        if (this._client == null)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_DEPORTATION_ACK(this.result));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_DEPORTATION_REQ: " + ex.ToString());
      }
    }

    private Message CreateMessage(PointBlank.Core.Models.Account.Clan.Clan clan, long owner, long senderId)
    {
      Message msg = new Message(15.0)
      {
        sender_name = clan._name,
        sender_id = senderId,
        clanId = clan._id,
        type = 4,
        state = 1,
        cB = NoteMessageClan.Deportation
      };
      if (!MessageManager.CreateMessage(owner, msg))
        return (Message) null;
      return msg;
    }
  }
}
