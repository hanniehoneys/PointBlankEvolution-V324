using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Data.Chat
{
  public static class HelpCommandList
  {
    public static string GetList3(Account player)
    {
      if (player.access < AccessLevel.Moderator)
        return Translation.GetLabel("HelpListNoLevel");
      if (HelpCommandList.InGame(player))
        return Translation.GetLabel("InGameBlock");
      string msg = Translation.GetLabel("HelpListTitle3") + "\n" + Translation.GetLabel("NickHistoryByID") + "\n" + Translation.GetLabel("IDHistoryByNick") + "\n" + Translation.GetLabel("FakeRank") + "\n" + Translation.GetLabel("ChangeNick") + "\n" + Translation.GetLabel("KickPlayer") + "\n" + Translation.GetLabel("EnableDisableGMColor") + "\n" + Translation.GetLabel("AntiKickActive") + "\n" + Translation.GetLabel("RoomUnlock") + "\n" + Translation.GetLabel("AFKCounter") + "\n" + Translation.GetLabel("AFKKick") + "\n" + Translation.GetLabel("PlayersCountInServer") + "\n" + Translation.GetLabel("PlayersCountInServer2") + "\n" + Translation.GetLabel("Ping");
      player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg));
      return Translation.GetLabel("HelpListList3");
    }

    public static string GetList4(Account player)
    {
      if (player.access < AccessLevel.GameMaster)
        return Translation.GetLabel("HelpListNoLevel");
      if (HelpCommandList.InGame(player))
        return Translation.GetLabel("InGameBlock");
      string msg = Translation.GetLabel("HelpListTitle4") + "\n\n" + Translation.GetLabel("MsgToAllServer") + "\n" + Translation.GetLabel("MsgToAllRoom") + "\n" + Translation.GetLabel("ChangeMapId") + "\n" + Translation.GetLabel("ChangeRoomTime") + "\n" + Translation.GetLabel("Give10Cash") + "\n" + Translation.GetLabel("Give10Gold") + "\n" + Translation.GetLabel("KickAll") + "\n" + Translation.GetLabel("SendGift") + "\n" + Translation.GetLabel("GoodsFound") + "\n" + Translation.GetLabel("SimpleBanNormal") + "\n" + Translation.GetLabel("AdvancedBanNormal") + "\n" + Translation.GetLabel("UnbanNormal") + "\n" + Translation.GetLabel("GetPlayersByIP") + "\n" + Translation.GetLabel("BanReason") + "\n" + Translation.GetLabel("GetPlayerInfos") + "\n" + Translation.GetLabel("OpenRoomSlot") + "\n" + Translation.GetLabel("OpenRandomRoomSlot") + "\n" + Translation.GetLabel("OpenAllClosedRoomSlots") + "\n" + Translation.GetLabel("TakeTitles");
      player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg));
      return Translation.GetLabel("HelpListList4");
    }

    public static string GetList5(Account player)
    {
      if (player.access < AccessLevel.Admin)
        return Translation.GetLabel("HelpListNoLevel");
      if (HelpCommandList.InGame(player))
        return Translation.GetLabel("InGameBlock");
      string msg = Translation.GetLabel("HelpListTitle5") + "\n\n" + Translation.GetLabel("ChangeRank") + "\n" + Translation.GetLabel("SimpleBanEtern") + "\n" + Translation.GetLabel("AdvancedBanEtern") + "\n" + Translation.GetLabel("GetBanInfo") + "\n" + Translation.GetLabel("UnbanEtern") + "\n" + Translation.GetLabel("CreateItemPt1") + "\n" + Translation.GetLabel("CreateItemPt2") + "\n" + Translation.GetLabel("CreateGoldItem") + "\n" + Translation.GetLabel("ReloadShop") + "\n" + Translation.GetLabel("V2ReloadShop") + "\n" + Translation.GetLabel("ChangeAnnounce") + "\n" + Translation.GetLabel("SetCashD") + "\n" + Translation.GetLabel("SetGoldD") + "\n" + Translation.GetLabel("CashPlayerD") + "\n" + Translation.GetLabel("GoldPlayerD") + "\n" + Translation.GetLabel("SetVip") + "\n" + Translation.GetLabel("SetAcess");
      player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg));
      return Translation.GetLabel("HelpListList5");
    }

    public static string GetList6(Account player)
    {
      if (player.access < AccessLevel.Devolper)
        return Translation.GetLabel("HelpListNoLevel");
      if (HelpCommandList.InGame(player))
        return Translation.GetLabel("InGameBlock");
      string msg = Translation.GetLabel("HelpListTitle6") + "\n\n" + Translation.GetLabel("Pausar") + "\n" + Translation.GetLabel("EndRoom") + "\n" + Translation.GetLabel("ChangeRoomType") + "\n" + Translation.GetLabel("ChangeRoomSpecial") + "\n" + Translation.GetLabel("ChangeRoomWeapons") + "\n" + Translation.GetLabel("ChangeUDP") + "\n" + Translation.GetLabel("EnableTestMode") + "\n" + Translation.GetLabel("EnablePublicMode") + "\n" + Translation.GetLabel("EnableMissions");
      player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg));
      return Translation.GetLabel("HelpListList6");
    }

    private static bool InGame(Account player)
    {
      PointBlank.Game.Data.Model.Room room = player._room;
      Slot slot;
      return room != null && room.getSlot(player._slotId, out slot) && slot.state >= SlotState.READY;
    }
  }
}
