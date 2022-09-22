using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_INVITE_ACCEPT_REQ : ReceivePacket
  {
    private int clanId;
    private int type;

    public PROTOCOL_CS_INVITE_ACCEPT_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.clanId = this.readD();
      this.readD();
      this.type = (int) this.readC();
    }

    public override void run()
    {
      PointBlank.Game.Data.Model.Account player = this._client._player;
      if (player == null || player.player_name.Length == 0)
        return;
      PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(this.clanId);
      List<PointBlank.Game.Data.Model.Account> clanPlayers = ClanManager.getClanPlayers(this.clanId, -1L, true);
      if (clan._id == 0)
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_INVITE_ACCEPT_ACK(2147487835U));
      else if (player.clanId > 0)
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_INVITE_ACCEPT_ACK(2147487832U));
      else if (clan.maxPlayers <= clanPlayers.Count)
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_INVITE_ACCEPT_ACK(2147487830U));
      }
      else
      {
        if (this.type != 0 && this.type != 1)
          return;
        try
        {
          uint erro = 0;
          PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(clan.owner_id, 0);
          if (account != null)
          {
            if (MessageManager.getMsgsCount(clan.owner_id) < 100)
            {
              Message message = this.CreateMessage(clan, player.player_name, this._client.player_id);
              if (message != null && account._isOnline)
                account.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(message), false);
            }
            if (this.type == 1)
            {
              int num = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
              if (ComDiv.updateDB("accounts", "player_id", (object) player.player_id, new string[3]
              {
                "clan_id",
                "clanaccess",
                "clandate"
              }, (object) clan._id, (object) 3, (object) num))
              {
                using (PROTOCOL_CS_MEMBER_INFO_INSERT_ACK memberInfoInsertAck = new PROTOCOL_CS_MEMBER_INFO_INSERT_ACK(player))
                  ClanManager.SendPacket((SendPacket) memberInfoInsertAck, clanPlayers);
                player.clanId = clan._id;
                player.clanDate = num;
                player.clanAccess = 3;
                this._client.SendPacket((SendPacket) new PROTOCOL_CS_MEMBER_INFO_ACK(clanPlayers));
                player._room?.SendPacketToPlayers((SendPacket) new PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(player, clan));
                this._client.SendPacket((SendPacket) new PROTOCOL_CS_ACCEPT_REQUEST_RESULT_ACK(clan, account, clanPlayers.Count + 1));
              }
              else
                erro = 2147483648U;
            }
          }
          else
            erro = 2147483648U;
          this._client.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_SEND_ACK(erro));
        }
        catch (Exception ex)
        {
          Logger.error(ex.ToString());
        }
      }
    }

    private Message CreateMessage(PointBlank.Core.Models.Account.Clan.Clan clan, string player, long senderId)
    {
      Message msg = new Message(15.0)
      {
        sender_name = clan._name,
        sender_id = senderId,
        clanId = clan._id,
        type = 4,
        text = player,
        state = 1,
        cB = this.type == 0 ? NoteMessageClan.JoinDenial : NoteMessageClan.JoinAccept
      };
      if (!MessageManager.CreateMessage(clan.owner_id, msg))
        return (Message) null;
      return msg;
    }
  }
}
