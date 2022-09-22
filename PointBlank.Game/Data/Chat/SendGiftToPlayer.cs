using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Shop;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Data.Chat
{
  public static class SendGiftToPlayer
  {
    public static string SendGiftById(string str)
    {
      if (!GameManager.Config.GiftSystem)
        return Translation.GetLabel("SendGift_SystemOffline");
      string[] strArray = str.Substring(str.IndexOf(" ") + 1).Split(' ');
      long int64 = Convert.ToInt64(strArray[0]);
      int int32 = Convert.ToInt32(strArray[1]);
      PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(int64, 0);
      if (account == null)
        return Translation.GetLabel("SendGift_Fail4");
      GoodItem good = ShopManager.getGood(int32);
      if (good != null && (good.visibility == 0 || good.visibility == 4))
      {
        Message message = new Message(30.0)
        {
          sender_name = "GM",
          sender_id = (long) int32,
          state = 0,
          type = 2
        };
        if (!MessageManager.CreateMessage(int64, message))
          return Translation.GetLabel("SendGift_Fail1");
        account.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_RECV_GIFT_ACK(message), false);
        return Translation.GetLabel("SendGift_Success", (object) good._item._name, (object) account.player_name);
      }
      if (good == null)
        return Translation.GetLabel("SendGift_Fail2");
      return Translation.GetLabel("SendGift_Fail3", (object) good._item._name);
    }
  }
}
