using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Data.Chat
{
  public static class ChangeRoomInfos
  {
    public static string ChangeMap(string str, Room room)
    {
      if (room == null)
        return Translation.GetLabel("GeneralRoomInvalid");
      if (room.isStartingMatch())
        return Translation.GetLabel("ChangeMapRoomFail");
      int num = int.Parse(str.Substring(4));
      if (num <= 0)
        return Translation.GetLabel("ChangeMapFail");
      room.mapId = (MapIdEnum) num;
      room.updateRoomInfo();
      return Translation.GetLabel("ChangeMapSuccess", (object) room.mapId);
    }

    public static string ChangeMap2(string str, Room room)
    {
      if (room == null)
        return Translation.GetLabel("GeneralRoomInvalid");
      if (room == null)
        return Translation.GetLabel("ChangeMapRoomFail");
      int num = int.Parse(str.Substring(4));
      if (num <= 0)
        return Translation.GetLabel("ChangeMapFail");
      room.mapId = (MapIdEnum) num;
      room.updateRoomInfo();
      return Translation.GetLabel("ChangeMapSuccess", (object) room.mapId);
    }

    public static string ChangeTime(string str, Room room)
    {
      if (room == null)
        return Translation.GetLabel("GeneralRoomInvalid");
      int num = int.Parse(str.Substring(2));
      if (num < 0)
        return Translation.GetLabel("ChangeTimeWrong");
      if (room.isStartingMatch())
        return Translation.GetLabel("ChangeTimeRoomFail");
      room.killtime = num;
      room.updateRoomInfo();
      return Translation.GetLabel("ChangeTimeSuccess", (object) room.getTimeByMask(), (object) room.getRoundsByMask(), (object) room.getKillsByMask());
    }

    public static string ChangeTime2(string str, Room room)
    {
      if (room == null)
        return Translation.GetLabel("GeneralRoomInvalid");
      uint num = uint.Parse(str.Substring(2));
      if (num < 0U)
        return Translation.GetLabel("ChangeTimeWrong");
      if (room == null)
        return Translation.GetLabel("ChangeTimeRoomFail");
      room._timeRoom = num;
      room.updateRoomInfo();
      return Translation.GetLabel("ChangeTimeSuccess", (object) room.getTimeByMask(), (object) room.getRoundsByMask(), (object) room.getKillsByMask());
    }

    public static string ChangeStageType(string str, Room room)
    {
      int num = int.Parse(str.Substring(12));
      if (room == null)
        return Translation.GetLabel("GeneralRoomInvalid");
      room.room_type = (RoomType) num;
      room.updateRoomInfo();
      return Translation.GetLabel("ChangeStageTypeSuccess", (object) (RoomType) num);
    }

    public static string ChangeWeaponsFlag(string str, Room room)
    {
      int num = int.Parse(str.Substring(12));
      if (room == null)
        return Translation.GetLabel("GeneralRoomInvalid");
      room.weaponsFlag = (RoomWeaponsFlag) num;
      room.updateRoomInfo();
      return Translation.GetLabel("ChangeWeaponsFlagSuccess", (object) (RoomWeaponsFlag) num);
    }

    public static string UnlockById(string str, Account player)
    {
      int id = int.Parse(str.Substring(11)) - 1;
      if (player == null)
        return Translation.GetLabel("RoomUnlock_Fail3");
      Channel channel = player.getChannel();
      if (channel == null)
        return Translation.GetLabel("GeneralChannelInvalid");
      Room room = channel.getRoom(id);
      if (room != null && room.Limit == (byte) 1)
      {
        room.Limit = (byte) 0;
        room.updateRoomInfo();
        return Translation.GetLabel("RoomUnlock_Success", (object) string.Format("{0:0##}", (object) (id + 1)));
      }
      return Translation.GetLabel("RoomUnlock_Fail1", (object) string.Format("{0:0##}", (object) (id + 1)));
    }
  }
}
