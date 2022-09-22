using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_LOBBY_LEAVE_REQ : ReceivePacket
  {
    private uint erro;

    public PROTOCOL_LOBBY_LEAVE_REQ(GameClient client, byte[] data)
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
        if (this._client == null || this._client._player == null)
          return;
        Account player = this._client._player;
        Channel channel = player.getChannel();
        if (player._room != null || player._match != null)
          return;
        if (channel == null || player.Session == null || !channel.RemovePlayer(player))
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_LOBBY_LEAVE_ACK(this.erro));
        if (this.erro == 0U)
        {
          player.ResetPages();
          player._status.updateChannel(byte.MaxValue);
          AllUtils.syncPlayerToFriends(player, false);
          AllUtils.syncPlayerToClanMembers(player);
        }
        else
          this._client.Close(1000, false);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_LOBBY_LEAVE_REQ: " + ex.ToString());
        this._client.SendPacket((SendPacket) new PROTOCOL_LOBBY_LEAVE_ACK(2147483648U));
        this._client.Close(1000, false);
      }
    }
  }
}
