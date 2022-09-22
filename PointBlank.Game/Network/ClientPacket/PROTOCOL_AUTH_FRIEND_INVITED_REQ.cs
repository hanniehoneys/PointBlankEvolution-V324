using PointBlank.Core;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_FRIEND_INVITED_REQ : ReceivePacket
  {
    private string playerName;
    private int idx1;
    private int idx2;

    public PROTOCOL_AUTH_FRIEND_INVITED_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.playerName = this.readUnicode(66);
    }

    public override void run()
    {
      try
      {
        if (this._client == null)
          return;
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null || player.player_name.Length == 0 || player.player_name == this.playerName)
          this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147487799U));
        else if (player.FriendSystem._friends.Count >= 50)
        {
          this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147487800U));
        }
        else
        {
          PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(this.playerName, 1, 32);
          if (account != null)
          {
            if (player.FriendSystem.GetFriendIdx(account.player_id) == -1)
            {
              if (account.FriendSystem._friends.Count >= 50)
              {
                this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147487800U));
              }
              else
              {
                int num = AllUtils.AddFriend(account, player, 2);
                if (AllUtils.AddFriend(player, account, num == 1 ? 0 : 1) == -1 || num == -1)
                {
                  this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147487801U));
                }
                else
                {
                  Friend friend1 = player.FriendSystem.GetFriend(account.player_id, out this.idx1);
                  Friend friend2 = account.FriendSystem.GetFriend(player.player_id, out this.idx2);
                  if (friend2 != null)
                    account.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(num == 0 ? FriendChangeState.Insert : FriendChangeState.Update, friend2, this.idx2), false);
                  if (friend1 == null)
                    return;
                  this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(FriendChangeState.Insert, friend1, this.idx1));
                }
              }
            }
            else
              this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147487809U));
          }
          else
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INVITED_ACK(2147487810U));
        }
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_AUTH_FRIEND_INVITED_REQ: " + ex.ToString());
      }
    }
  }
}
