using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Data.Chat
{
  public static class LatencyAnalyze
  {
    public static string StartAnalyze(Account player, Room room)
    {
      if (room == null)
        return Translation.GetLabel("GeneralRoomInvalid");
      if (room.getSlot(player._slotId).state != SlotState.BATTLE)
        return Translation.GetLabel("LatencyInfoError");
      player.DebugPing = !player.DebugPing;
      if (player.DebugPing)
        return Translation.GetLabel("LatencyInfoOn");
      return Translation.GetLabel("LatencyInfoOff");
    }
  }
}
