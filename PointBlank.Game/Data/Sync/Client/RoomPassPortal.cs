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
  public static class RoomPassPortal
  {
    public static void Load(ReceiveGPacket p)
    {
      int id1 = (int) p.readH();
      int id2 = (int) p.readH();
      int slotIdx = (int) p.readC();
      int num = (int) p.readC();
      Channel channel = ChannelsXml.getChannel(id2);
      if (channel == null)
        return;
      PointBlank.Game.Data.Model.Room room = channel.getRoom(id1);
      if (room != null && room.round.Timer == null && (room._state == RoomState.Battle && room.room_type == RoomType.Boss))
      {
        Slot slot = room.getSlot(slotIdx);
        if (slot != null && slot.state == SlotState.BATTLE)
        {
          ++slot.passSequence;
          if (slot._team == 0)
            room.red_dino += 5;
          else
            room.blue_dino += 5;
          RoomPassPortal.CompleteMission(room, slot);
          using (PROTOCOL_BATTLE_MISSION_TOUCHDOWN_ACK missionTouchdownAck = new PROTOCOL_BATTLE_MISSION_TOUCHDOWN_ACK(room, slot))
          {
            using (PROTOCOL_BATTLE_MISSION_TOUCHDOWN_COUNT_ACK touchdownCountAck = new PROTOCOL_BATTLE_MISSION_TOUCHDOWN_COUNT_ACK(room))
              room.SendPacketToPlayers((SendPacket) missionTouchdownAck, (SendPacket) touchdownCountAck, SlotState.BATTLE, 0);
          }
        }
      }
      if (p.getBuffer().Length <= 8)
        return;
      Logger.warning("Invalid Portal: " + BitConverter.ToString(p.getBuffer()));
    }

    private static void CompleteMission(PointBlank.Game.Data.Model.Room room, Slot slot)
    {
      MissionType autoComplete = MissionType.NA;
      if (slot.passSequence == 1)
        autoComplete = MissionType.TOUCHDOWN;
      else if (slot.passSequence == 2)
        autoComplete = MissionType.TOUCHDOWN_ACE_ATTACKER;
      else if (slot.passSequence == 3)
        autoComplete = MissionType.TOUCHDOWN_HATTRICK;
      else if (slot.passSequence >= 4)
        autoComplete = MissionType.TOUCHDOWN_GAME_MAKER;
      if (autoComplete == MissionType.NA)
        return;
      AllUtils.CompleteMission(room, slot, autoComplete, 0);
    }
  }
}
