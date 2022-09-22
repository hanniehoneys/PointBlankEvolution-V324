using PointBlank.Core;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_FRIEND_INVITED_REQUEST_REQ : ReceivePacket
  {
    private int index;

    public PROTOCOL_AUTH_FRIEND_INVITED_REQUEST_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.index = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        PointBlank.Game.Data.Model.Account friend = this.GetFriend(player);
        if (friend != null)
        {
          if (friend._status.serverId == byte.MaxValue || friend._status.serverId == (byte) 0)
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147495938U));
          else if (friend.matchSlot >= 0)
          {
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147495939U));
          }
          else
          {
            int friendIdx = friend.FriendSystem.GetFriendIdx(player.player_id);
            if (friendIdx == -1)
              this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147487806U));
            else if (friend._isOnline)
              friend.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_REQUEST_ACK(friendIdx), false);
            else
              this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147487807U));
          }
        }
        else
          this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147487805U));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_AUTH_FRIEND_INVITED_REQUEST_REQ " + ex.ToString());
      }
    }

    private PointBlank.Game.Data.Model.Account GetFriend(PointBlank.Game.Data.Model.Account p)
    {
      Friend friend = p.FriendSystem.GetFriend(this.index);
      if (friend == null)
        return (PointBlank.Game.Data.Model.Account) null;
      return AccountManager.getAccount(friend.player_id, 32);
    }
  }
}
