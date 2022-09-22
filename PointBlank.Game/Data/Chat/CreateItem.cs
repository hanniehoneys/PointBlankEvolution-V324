using PointBlank.Core;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Data.Chat
{
  public static class CreateItem
  {
    public static string CreateItemYourself(string str, PointBlank.Game.Data.Model.Account player)
    {
      int id = int.Parse(str.Substring(3));
      if (id < 100000)
        return Translation.GetLabel("CreateItemWrongID");
      if (player == null)
        return Translation.GetLabel("CreateItemFail");
      int itemCategory = ComDiv.GetItemCategory(id);
      player.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, new ItemsModel(id, itemCategory, "Command Item", itemCategory == 3 ? 1 : 3, 1L, 0L)));
      return Translation.GetLabel("CreateItemSuccess");
    }

    public static string CreateItemByNick(string str, PointBlank.Game.Data.Model.Account player)
    {
      string[] strArray = str.Substring(str.IndexOf(" ") + 1).Split(' ');
      string text = strArray[0];
      int int32 = Convert.ToInt32(strArray[1]);
      if (int32 < 100000)
        return Translation.GetLabel("CreateItemWrongID");
      PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(text, 1, 0);
      if (account == null)
        return Translation.GetLabel("CreateItemFail");
      if (account.player_id == player.player_id)
        return Translation.GetLabel("CreateItemUseOtherCMD");
      int itemCategory = ComDiv.GetItemCategory(int32);
      account.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, account, new ItemsModel(int32, itemCategory, "Command Item", itemCategory == 3 ? 1 : 3, 1L, 0L)), false);
      return Translation.GetLabel("CreateItemSuccess");
    }

    public static string CreateItemById(string str, PointBlank.Game.Data.Model.Account player)
    {
      string[] strArray = str.Substring(str.IndexOf(" ") + 1).Split(' ');
      int int32 = Convert.ToInt32(strArray[1]);
      long int64 = Convert.ToInt64(strArray[0]);
      if (int32 < 100000)
        return Translation.GetLabel("CreateItemWrongID");
      PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(int64, 0);
      if (account == null)
        return Translation.GetLabel("CreateItemFail");
      if (account.player_id == player.player_id)
        return Translation.GetLabel("CreateItemUseOtherCMD");
      int itemCategory = ComDiv.GetItemCategory(int32);
      account.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, account, new ItemsModel(int32, itemCategory, "Command Item", itemCategory == 3 ? 1 : 3, 1L, 0L)), false);
      return Translation.GetLabel("CreateItemSuccess");
    }

    public static string CreateGoldCupom(string str)
    {
      string[] strArray = str.Substring(str.IndexOf(" ") + 1).Split(' ');
      int int32 = Convert.ToInt32(strArray[1]);
      long int64 = Convert.ToInt64(strArray[0]);
      if (!int32.ToString().EndsWith("00"))
        return Translation.GetLabel("CreateSItemFail");
      if (int32 < 100 || int32 > 99999999)
        return Translation.GetLabel("CreateSItemWrongID");
      PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(int64, 0);
      if (account == null)
        return Translation.GetLabel("CreateItemFail");
      int itemId = ComDiv.CreateItemId(20, int32 % 100000 / 1000, int32 % 1000);
      account.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, account, new ItemsModel(itemId, 3, "Gold Item", 1, 1L, 0L)), false);
      return Translation.GetLabel("CreateSItemSuccess", (object) int32);
    }
  }
}
