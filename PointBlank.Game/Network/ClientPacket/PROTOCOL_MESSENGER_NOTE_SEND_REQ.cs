using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_MESSENGER_NOTE_SEND_REQ : ReceivePacket
  {
    private int nameLength;
    private int textLength;
    private string name;
    private string text;
    private uint erro;

    public PROTOCOL_MESSENGER_NOTE_SEND_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.nameLength = (int) this.readC();
      this.textLength = (int) this.readC();
      this.name = this.readUnicode(this.nameLength * 2);
      this.text = this.readUnicode(this.textLength * 2);
    }

    public override void run()
    {
      try
      {
        if (this.text.Length > 120)
          return;
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null || player.player_name.Length == 0 || player.player_name == this.name)
          return;
        PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(this.name, 1, 0);
        if (account != null)
        {
          if (MessageManager.getMsgsCount(account.player_id) >= 100)
          {
            this.erro = 2147487871U;
          }
          else
          {
            Message message = this.CreateMessage(player.player_name, account.player_id, this._client.player_id);
            if (message != null)
              account.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(message), false);
          }
        }
        else
          this.erro = 2147487870U;
        this._client.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_SEND_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_MESSENGER_NOTE_SEND_REQ: " + ex.ToString());
      }
    }

    private Message CreateMessage(string senderName, long owner, long senderId)
    {
      Message msg = new Message(15.0)
      {
        sender_name = senderName,
        sender_id = senderId,
        text = this.text,
        state = 1
      };
      if (MessageManager.CreateMessage(owner, msg))
        return msg;
      this.erro = 2147483648U;
      return (Message) null;
    }
  }
}
