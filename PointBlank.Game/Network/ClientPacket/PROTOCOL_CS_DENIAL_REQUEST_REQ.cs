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
  public class PROTOCOL_CS_DENIAL_REQUEST_REQ : ReceivePacket
  {
    private int result;

    public PROTOCOL_CS_DENIAL_REQUEST_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      PointBlank.Game.Data.Model.Account player = this._client._player;
      if (player == null)
        return;
      PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
      if (clan._id <= 0 || (player.clanAccess < 1 || player.clanAccess > 2) && clan.owner_id != player.player_id)
        return;
      int num1 = (int) this.readC();
      for (int index = 0; index < num1; ++index)
      {
        long num2 = this.readQ();
        if (PlayerManager.DeleteInviteDb(clan._id, num2))
        {
          if (MessageManager.getMsgsCount(num2) < 100)
          {
            Message message = this.CreateMessage(clan, num2, player.player_id);
            if (message != null)
            {
              PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(num2, 0);
              if (account != null && account._isOnline)
                account.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(message), false);
            }
          }
          ++this.result;
        }
      }
    }

    public override void run()
    {
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_DENIAL_REQUEST_ACK(this.result));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_DENIAL_REQUEST_REQ: " + ex.ToString());
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
        cB = NoteMessageClan.InviteDenial
      };
      if (!MessageManager.CreateMessage(owner, msg))
        return (Message) null;
      return msg;
    }
  }
}
