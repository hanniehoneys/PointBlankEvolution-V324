using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_GET_LOBBY_USER_LIST_REQ : ReceivePacket
  {
    public PROTOCOL_ROOM_GET_LOBBY_USER_LIST_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      Account player = this._client._player;
      if (player == null)
        return;
      try
      {
        Channel channel = player.getChannel();
        if (channel == null)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_GET_LOBBY_USER_LIST_ACK(channel));
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_ROOM_GET_LOBBY_USER_LIST_REQ: " + ex.ToString());
      }
    }
  }
}
