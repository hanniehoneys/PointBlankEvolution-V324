using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Data.Chat
{
  public static class GetRoomInfo
  {
    public static string GetSlotStats(string str, Account player, PointBlank.Game.Data.Model.Room room)
    {
      int slotIdx = int.Parse(str.Substring(5)) - 1;
      string str1 = "Infomation:";
      if (room == null)
        return "Sala invalid. [Server]";
      Slot slot = room.getSlot(slotIdx);
      if (slot == null)
        return "Slot invalid. [Server]";
      string msg = str1 + "\nIndex: " + (object) slot._id + "\nTeam: " + (object) slot._team + "\nFlag: " + (object) slot.Flag + "\nAccountId: " + (object) slot._playerId + "\nState: " + (object) slot.state + "\nMissions: " + (slot.Missions != null ? "Valid" : "Null");
      player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg));
      return "Successfully generated slot logs. [Server]";
    }
  }
}
