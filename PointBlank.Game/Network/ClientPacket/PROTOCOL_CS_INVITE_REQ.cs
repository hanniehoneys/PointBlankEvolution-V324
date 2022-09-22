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
  public class PROTOCOL_CS_INVITE_REQ : ReceivePacket
  {
    private uint erro;

    public PROTOCOL_CS_INVITE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      PointBlank.Game.Data.Model.Account player = this._client._player;
      if (player == null)
        return;
      if (player.clanId == 0)
        return;
      try
      {
        if (player.FindPlayer == "" || player.FindPlayer.Length == 0)
          return;
        PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(player.FindPlayer, 1, 0);
        if (account != null)
        {
          if (account.clanId == 0 && player.clanId != 0)
            this.SendBoxMessage(account, player.clanId);
          else
            this.erro = 2147483648U;
        }
        else
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_INVITE_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    private void SendBoxMessage(PointBlank.Game.Data.Model.Account player, int clanId)
    {
      if (MessageManager.getMsgsCount(player.player_id) >= 100)
      {
        this.erro = 2147483648U;
      }
      else
      {
        Message message = this.CreateMessage(clanId, player.player_id, this._client.player_id);
        if (message == null || !player._isOnline)
          return;
        player.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(message), false);
      }
    }

    private Message CreateMessage(int clanId, long owner, long senderId)
    {
      Message msg = new Message(15.0)
      {
        sender_name = ClanManager.getClan(clanId)._name,
        clanId = clanId,
        sender_id = senderId,
        type = 5,
        state = 1,
        cB = NoteMessageClan.Invite
      };
      if (!MessageManager.CreateMessage(owner, msg))
        return (Message) null;
      return msg;
    }
  }
}
