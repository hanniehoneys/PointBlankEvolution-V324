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
  public class PROTOCOL_CS_ACCEPT_REQUEST_REQ : ReceivePacket
  {
    private int result;

    public PROTOCOL_CS_ACCEPT_REQUEST_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      PointBlank.Game.Data.Model.Account player = this._client._player;
      if (player == null)
        return;
      PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
      if (clan._id > 0 && (player.clanAccess >= 1 && player.clanAccess <= 2 || player.player_id == clan.owner_id))
      {
        List<PointBlank.Game.Data.Model.Account> clanPlayers = ClanManager.getClanPlayers(clan._id, -1L, true);
        if (clanPlayers.Count >= clan.maxPlayers)
        {
          this.result = -1;
        }
        else
        {
          int num = (int) this.readC();
          for (int index = 0; index < num; ++index)
          {
            PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(this.readQ(), 0);
            if (account != null && clanPlayers.Count < clan.maxPlayers && (account.clanId == 0 && PlayerManager.getRequestClanId(account.player_id) > 0))
            {
              using (PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK memberInfoChangeAck = new PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK(account))
                ClanManager.SendPacket((SendPacket) memberInfoChangeAck, clanPlayers);
              account.clanId = player.clanId;
              account.clanDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
              account.clanAccess = 3;
              SendClanInfo.Load(account, (PointBlank.Game.Data.Model.Account) null, 3);
              ComDiv.updateDB("accounts", "player_id", (object) account.player_id, new string[3]
              {
                "clanaccess",
                "clan_id",
                "clandate"
              }, (object) account.clanAccess, (object) account.clanId, (object) account.clanDate);
              PlayerManager.DeleteInviteDb(player.clanId, account.player_id);
              if (account._isOnline)
              {
                account.SendPacket((SendPacket) new PROTOCOL_CS_MEMBER_INFO_ACK(clanPlayers), false);
                account._room?.SendPacketToPlayers((SendPacket) new PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(account, clan));
                account.SendPacket((SendPacket) new PROTOCOL_CS_ACCEPT_REQUEST_RESULT_ACK(clan, clanPlayers.Count + 1), false);
              }
              if (MessageManager.getMsgsCount(account.player_id) < 100)
              {
                Message message = this.CreateMessage(clan, account.player_id, this._client.player_id);
                if (message != null && account._isOnline)
                  account.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(message), false);
              }
              ++this.result;
              clanPlayers.Add(account);
            }
          }
        }
      }
      else
        this.result = -1;
    }

    public override void run()
    {
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_ACCEPT_REQUEST_ACK((uint) this.result));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_ACCEPT_REQUEST_RESULT_REQ: " + ex.ToString());
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
        cB = NoteMessageClan.InviteAccept
      };
      if (!MessageManager.CreateMessage(owner, msg))
        return (Message) null;
      return msg;
    }
  }
}
