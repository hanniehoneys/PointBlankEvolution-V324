using PointBlank.Auth.Data.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Network;

namespace PointBlank.Auth.Data.Sync.Client
{
  public static class FriendSync
  {
    public static void Load(ReceiveGPacket p)
    {
      long id = p.readQ();
      int num1 = (int) p.readC();
      long num2 = p.readQ();
      Friend friend = (Friend) null;
      if (num1 <= 1)
      {
        int num3 = (int) p.readC();
        bool flag = p.readC() == (byte) 1;
        friend = new Friend(num2)
        {
          state = num3,
          removed = flag
        };
      }
      if (friend == null && num1 <= 1)
        return;
      PointBlank.Auth.Data.Model.Account account = AccountManager.getInstance().getAccount(id, true);
      if (account == null)
        return;
      if (num1 <= 1)
      {
        friend.player.player_name = account.player_name;
        friend.player._rank = account._rank;
        friend.player._isOnline = account._isOnline;
        friend.player._status = account._status;
      }
      if (num1 == 0)
        account.FriendSystem.AddFriend(friend);
      else if (num1 == 1)
      {
        if (account.FriendSystem.GetFriend(num2) == null);
      }
      else
      {
        if (num1 != 2)
          return;
        account.FriendSystem.RemoveFriend(num2);
      }
    }
  }
}
