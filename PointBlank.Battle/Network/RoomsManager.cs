using PointBlank.Battle.Data;
using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Models;
using System;
using System.Collections.Generic;

namespace PointBlank.Battle.Network
{
  public class RoomsManager
  {
    private static List<Room> list = new List<Room>();

    public static Room CreateOrGetRoom(uint UniqueRoomId, uint Seed)
    {
      lock (RoomsManager.list)
      {
        for (int index = 0; index < RoomsManager.list.Count; ++index)
        {
          Room room = RoomsManager.list[index];
          if ((int) room.UniqueRoomId == (int) UniqueRoomId)
            return room;
        }
        int roomInfo1 = AllUtils.GetRoomInfo(UniqueRoomId, 2);
        int roomInfo2 = AllUtils.GetRoomInfo(UniqueRoomId, 1);
        int roomInfo3 = AllUtils.GetRoomInfo(UniqueRoomId, 0);
        Room room1 = new Room(roomInfo1) { UniqueRoomId = UniqueRoomId, Seed = Seed, RoomId = roomInfo3, ChannelId = roomInfo2, MapId = (MAP_STATE_ID) AllUtils.GetSeedInfo(Seed, 2), RoomType = (ROOM_STATE_TYPE) AllUtils.GetSeedInfo(Seed, 0), Rule = AllUtils.GetSeedInfo(Seed, 1) };
        RoomsManager.list.Add(room1);
        return room1;
      }
    }

    public static Room getRoom(uint UniqueRoomId)
    {
      lock (RoomsManager.list)
      {
        for (int index = 0; index < RoomsManager.list.Count; ++index)
        {
          Room room = RoomsManager.list[index];
          if (room != null && (int) room.UniqueRoomId == (int) UniqueRoomId)
            return room;
        }
        return (Room) null;
      }
    }

    public static Room getRoom(uint UniqueRoomId, uint Seed)
    {
      lock (RoomsManager.list)
      {
        for (int index = 0; index < RoomsManager.list.Count; ++index)
        {
          Room room = RoomsManager.list[index];
          if (room != null && (int) room.UniqueRoomId == (int) UniqueRoomId && (int) room.Seed == (int) Seed)
            return room;
        }
        return (Room) null;
      }
    }

    public static bool getRoom(uint UniqueRoomId, out Room room)
    {
      room = (Room) null;
      lock (RoomsManager.list)
      {
        for (int index = 0; index < RoomsManager.list.Count; ++index)
        {
          Room room1 = RoomsManager.list[index];
          if (room1 != null && (int) room1.UniqueRoomId == (int) UniqueRoomId)
          {
            room = room1;
            return true;
          }
        }
      }
      return false;
    }

    public static void RemoveRoom(uint UniqueRoomId)
    {
      try
      {
        lock (RoomsManager.list)
        {
          for (int index = 0; index < RoomsManager.list.Count; ++index)
          {
            if ((int) RoomsManager.list[index].UniqueRoomId == (int) UniqueRoomId)
            {
              RoomsManager.list.RemoveAt(index);
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
    }
  }
}
