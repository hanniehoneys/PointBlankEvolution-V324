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
  public class PROTOCOL_AUTH_FRIEND_ACCEPT_REQ : ReceivePacket
  {
    private int index;
    private uint erro;

    public PROTOCOL_AUTH_FRIEND_ACCEPT_REQ(GameClient client, byte[] data)
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
        if (friend1 != null && friend1.state > 0)
        {
          PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(friend1.player_id, 32);
          if (account != null)
          {
            if (friend1.player == null)
              friend1.SetModel(account.player_id, account._rank, account.name_color, account.player_name, account._isOnline, account._status);
            else
              friend1.player.SetInfo(account._rank, account.name_color, account.player_name, account._isOnline, account._status);
            friend1.state = 0;
            PlayerManager.UpdateFriendState(player.player_id, friend1);
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(FriendChangeState.Accept, (Friend) null, 0, this.index));
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(FriendChangeState.Update, friend1, this.index));
            int index = -1;
            Friend friend2 = account.FriendSystem.GetFriend(player.player_id, out index);
            if (friend2 != null && friend2.state > 0)
            {
              if (friend2.player == null)
                friend2.SetModel(player.player_id, player._rank, player.name_color, player.player_name, player._isOnline, player._status);
              else
                friend2.player.SetInfo(player._rank, player.name_color, player.player_name, player._isOnline, player._status);
              friend2.state = 0;
              PlayerManager.UpdateFriendState(account.player_id, friend2);
              SendFriendInfo.Load(account, friend2, 1);
              account.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(FriendChangeState.Update, friend2, index), false);
            }
          }
          else
            this.erro = 2147483648U;
        }
        else
          this.erro = 2147483648U;
        if (this.erro <= 0U)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_ACCEPT_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_AUTH_FRIEND_ACCEPT_REQ " + ex.ToString());
      }
    }
  }
}
