using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Data.Sync.Client
{
  public static class RoomC4
  {
    public static void Load(ReceiveGPacket p)
    {
      int id1 = (int) p.readH();
      int id2 = (int) p.readH();
      int num = (int) p.readC();
      int slotIdx = (int) p.readC();
      int areaId = 0;
      float x = 0.0f;
      float y = 0.0f;
      float z = 0.0f;
      switch (num)
      {
        case 0:
          areaId = (int) p.readC();
          x = p.readT();
          y = p.readT();
          z = p.readT();
          if (p.getBuffer().Length > 21)
          {
            Logger.warning("Invalid Bomb: " + BitConverter.ToString(p.getBuffer()));
            break;
          }
          break;
        case 1:
          if (p.getBuffer().Length > 8)
          {
            Logger.warning("Invalid Bomb Type[1]: " + BitConverter.ToString(p.getBuffer()));
            break;
          }
          break;
      }
      Channel channel = ChannelsXml.getChannel(id2);
      if (channel == null)
        return;
      PointBlank.Game.Data.Model.Room room = channel.getRoom(id1);
      if (room == null || room.round.Timer != null || room._state != RoomState.Battle)
        return;
      Slot slot = room.getSlot(slotIdx);
      if (slot == null || slot.state != SlotState.BATTLE)
        return;
      if (num == 0)
      {
        RoomC4.InstallBomb(room, slot, areaId, x, y, z);
      }
      else
      {
        if (num != 1)
          return;
        RoomC4.UninstallBomb(room, slot);
      }
    }

    public static void InstallBomb(PointBlank.Game.Data.Model.Room room, Slot slot, int areaId, float x, float y, float z)
    {
      if (room.C4_actived)
        return;
      using (PROTOCOL_BATTLE_MISSION_BOMB_INSTALL_ACK missionBombInstallAck = new PROTOCOL_BATTLE_MISSION_BOMB_INSTALL_ACK(slot._id, (byte) areaId, x, y, z))
        room.SendPacketToPlayers((SendPacket) missionBombInstallAck, SlotState.BATTLE, 0);
      if (room.room_type != RoomType.Tutorial)
      {
        room.C4_actived = true;
        ++slot.objects;
        AllUtils.CompleteMission(room, slot, MissionType.C4_PLANT, 0);
        room.StartBomb();
      }
      else
        room.C4_actived = true;
    }

    public static void UninstallBomb(PointBlank.Game.Data.Model.Room room, Slot slot)
    {
      if (!room.C4_actived)
        return;
      using (PROTOCOL_BATTLE_MISSION_BOMB_UNINSTALL_ACK bombUninstallAck = new PROTOCOL_BATTLE_MISSION_BOMB_UNINSTALL_ACK(slot._id))
        room.SendPacketToPlayers((SendPacket) bombUninstallAck, SlotState.BATTLE, 0);
      if (room.room_type != RoomType.Tutorial)
      {
        ++slot.objects;
        ++room.blue_rounds;
        AllUtils.CompleteMission(room, slot, MissionType.C4_DEFUSE, 0);
        AllUtils.BattleEndRound(room, 1, RoundEndType.Uninstall);
      }
      else
        room.C4_actived = false;
    }
  }
}
