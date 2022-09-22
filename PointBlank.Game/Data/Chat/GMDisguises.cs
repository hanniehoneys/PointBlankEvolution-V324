using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Data.Chat
{
  public static class GMDisguises
  {
    public static string SetHideColor(Account player)
    {
      if (player == null)
        return Translation.GetLabel("HideGMColorFail");
      if (player._rank != 53 && player._rank != 54)
        return Translation.GetLabel("HideGMColorNoRank");
      player.HideGMcolor = !player.HideGMcolor;
      if (player.HideGMcolor)
        return Translation.GetLabel("HideGMColorSuccess1");
      return Translation.GetLabel("HideGMColorSuccess2");
    }

    public static string SetAntiKick(Account player)
    {
      if (player == null)
        return Translation.GetLabel("AntiKickGMFail");
      player.AntiKickGM = !player.AntiKickGM;
      if (player.AntiKickGM)
        return Translation.GetLabel("AntiKickGMSuccess1");
      return Translation.GetLabel("AntiKickGMSuccess2");
    }

    public static string SetFakeRank(string str, Account player, Room room)
    {
      int num = int.Parse(str.Substring(9));
      if (num > 55 || num < 0)
        return Translation.GetLabel("FakeRankWrongValue");
      if (player._bonus.fakeRank == num)
        return Translation.GetLabel("FakeRankAlreadyFaked");
      if (!ComDiv.updateDB("player_bonus", "fakerank", (object) num, "player_id", (object) player.player_id))
        return Translation.GetLabel("FakeRankFail");
      player._bonus.fakeRank = num;
      player.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(0, player));
      room?.updateSlotsInfo();
      if (num == 55)
        return Translation.GetLabel("FakeRankSuccess1");
      return Translation.GetLabel("FakeRankSuccess2", (object) num);
    }

    public static string SetFakeNick(string str, Account player, Room room)
    {
      string name = str.Substring(11);
      if (name.Length > GameConfig.maxNickSize || name.Length < GameConfig.minNickSize)
        return Translation.GetLabel("FakeNickWrongLength");
      if (PlayerManager.isPlayerNameExist(name))
        return Translation.GetLabel("FakeNickAlreadyExist");
      if (!ComDiv.updateDB("accounts", "player_name", (object) name, "player_id", (object) player.player_id))
        return Translation.GetLabel("FakeNickFail");
      player.player_name = name;
      player.SendPacket((SendPacket) new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(name));
      if (room != null)
      {
        using (PROTOCOL_ROOM_GET_NICKNAME_ACK roomGetNicknameAck = new PROTOCOL_ROOM_GET_NICKNAME_ACK(player._slotId, player.player_name, player.name_color))
          room.SendPacketToPlayers((SendPacket) roomGetNicknameAck);
      }
      if (player.clanId > 0)
      {
        using (PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK memberInfoChangeAck = new PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK(player))
          ClanManager.SendPacket((SendPacket) memberInfoChangeAck, player.clanId, -1L, true, true);
      }
      AllUtils.syncPlayerToFriends(player, true);
      return Translation.GetLabel("FakeNickSuccess", (object) name);
    }
  }
}
