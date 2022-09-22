using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Network;
using System;
using System.Collections.Generic;

namespace PointBlank.Auth.Network.ClientPacket
{
  public class PROTOCOL_BASE_GET_OPTION_REQ : ReceivePacket
  {
    public PROTOCOL_BASE_GET_OPTION_REQ(AuthClient client, byte[] data)
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
        PointBlank.Auth.Data.Model.Account player = this._client._player;
        if (player == null || player._myConfigsLoaded)
          return;
        this.SendMessagesList(player);
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_OPTION_ACK(0, player._config));
        if (player.FriendSystem._friends.Count <= 0)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_ACK(player.FriendSystem._friends));
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_BASE_GET_OPTION_REQ: " + ex.ToString());
      }
    }

    private void SendMessagesList(PointBlank.Auth.Data.Model.Account p)
    {
      List<Message> messages = MessageManager.getMessages(p.player_id);
      if (messages.Count == 0)
        return;
      MessageManager.RecicleMessages(p.player_id, messages);
      if (messages.Count == 0)
        return;
      int num = (int) Math.Ceiling((double) messages.Count / 25.0);
      for (int pageIdx = 0; pageIdx < num; ++pageIdx)
        this._client.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_LIST_ACK(pageIdx, messages));
    }
  }
}
