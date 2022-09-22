using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_FRIEND_DELETE_REQ : ReceivePacket
  {
    private int index;
    private uint erro;

    public PROTOCOL_AUTH_FRIEND_DELETE_REQ(GameClient client, byte[] data)
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
        Friend friend1 = player.FriendSystem.GetFriend(this.index);
        if (friend1 != null)
        {
          PlayerManager.DeleteFriend(friend1.player_id, player.player_id);
          PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(friend1.player_id, 32);
          if (account != null)
          {
            int index = -1;
            Friend friend2 = account.FriendSystem.GetFriend(player.player_id, out index);
            if (friend2 != null)
            {
              friend2.removed = true;
              PlayerManager.UpdateFriendBlock(account.player_id, friend2);
              SendFriendInfo.Load(account, friend2, 2);
              account.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(FriendChangeState.Update, friend2, index), false);
            }
          }
          player.FriendSystem.RemoveFriend(friend1);
          this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(FriendChangeState.Delete, (Friend) null, 0, this.index));
        }
        else
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_DELETE_ACK(this.erro));
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_ACK(player.FriendSystem._friends));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_AUTH_FRIEND_DELETE_REQ: " + ex.ToString());
      }
    }
  }
}
