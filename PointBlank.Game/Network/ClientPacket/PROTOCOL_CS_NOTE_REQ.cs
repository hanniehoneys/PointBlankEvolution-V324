using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_NOTE_REQ : ReceivePacket
  {
    private int type;
    private string message;

    public PROTOCOL_CS_NOTE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.type = (int) this.readC();
      this.message = this.readUnicode((int) this.readC() * 2);
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (this.message.Length > 120 || player == null)
          return;
        PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
        int count = 0;
        if (clan._id > 0 && clan.owner_id == this._client.player_id)
        {
          List<PointBlank.Game.Data.Model.Account> clanPlayers = ClanManager.getClanPlayers(clan._id, this._client.player_id, true);
          for (int index = 0; index < clanPlayers.Count; ++index)
          {
            PointBlank.Game.Data.Model.Account account = clanPlayers[index];
            if ((this.type == 0 || account.clanAccess == 2 && this.type == 1 || account.clanAccess == 3 && this.type == 2) && MessageManager.getMsgsCount(account.player_id) < 100)
            {
              ++count;
              Message message = this.CreateMessage(clan, account.player_id, this._client.player_id);
              if (message != null && account._isOnline)
                account.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(message), false);
            }
          }
        }
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_NOTE_ACK(count));
        if (count <= 0)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_SEND_ACK(0U));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_CS_NOTE_REQ: " + ex.ToString());
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
        text = this.message,
        state = 1
      };
      if (!MessageManager.CreateMessage(owner, msg))
        return (Message) null;
      return msg;
    }
  }
}
