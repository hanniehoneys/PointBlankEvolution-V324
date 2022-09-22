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
  public class PROTOCOL_CS_COMMISSION_MASTER_REQ : ReceivePacket
  {
    private long memberId;
    private uint erro;

    public PROTOCOL_CS_COMMISSION_MASTER_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.memberId = this.readQ();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null || player.clanAccess != 1)
          return;
        PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(this.memberId, 0);
        int clanId = player.clanId;
        if (account == null || account.clanId != clanId)
          this.erro = 2147483648U;
        else if (account._rank > 10)
        {
          PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(clanId);
          if (clan._id > 0 && clan.owner_id == this._client.player_id && (account.clanAccess == 2 && ComDiv.updateDB("clan_data", "owner_id", (object) this.memberId, "clan_id", (object) clanId)) && (ComDiv.updateDB("accounts", "clanaccess", (object) 1, "player_id", (object) this.memberId) && ComDiv.updateDB("accounts", "clanaccess", (object) 2, "player_id", (object) player.player_id)))
          {
            account.clanAccess = 1;
            player.clanAccess = 2;
            clan.owner_id = this.memberId;
            if (MessageManager.getMsgsCount(account.player_id) < 100)
            {
              Message message = this.CreateMessage(clan, account.player_id, player.player_id);
              if (message != null && account._isOnline)
                account.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(message), false);
            }
            if (account._isOnline)
              account.SendPacket((SendPacket) new PROTOCOL_CS_COMMISSION_MASTER_RESULT_ACK(), false);
          }
          else
            this.erro = 2147487744U;
        }
        else
          this.erro = 2147487928U;
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_COMMISSION_MASTER_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_COMMISSION_MASTER_REQ: " + ex.ToString());
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
        cB = NoteMessageClan.Master
      };
      if (!MessageManager.CreateMessage(owner, msg))
        return (Message) null;
      return msg;
    }
  }
}
