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
  public class RoomSabotageSync
  {
    public static void Load(ReceiveGPacket p)
    {
      int id1 = (int) p.readH();
      int id2 = (int) p.readH();
      byte num1 = p.readC();
      ushort num2 = p.readUH();
      ushort num3 = p.readUH();
      int num4 = (int) p.readC();
      ushort num5 = p.readUH();
      if (p.getBuffer().Length > 14)
        Logger.warning("Invalid Sabotage: " + BitConverter.ToString(p.getBuffer()));
      Channel channel = ChannelsXml.getChannel(id2);
      if (channel == null)
        return;
      PointBlank.Game.Data.Model.Room room = channel.getRoom(id1);
      Slot slot;
      if (room == null || room.round.Timer != null || (room._state != RoomState.Battle || room.swapRound) || !room.getSlot((int) num1, out slot))
        return;
      room.Bar1 = (int) num2;
      room.Bar2 = (int) num3;
      RoomType roomType = room.room_type;
      int num6 = 0;
      switch (num4)
      {
        case 1:
          slot.damageBar1 += num5;
          num6 += (int) slot.damageBar1 / 600;
          break;
        case 2:
          slot.damageBar2 += num5;
          num6 += (int) slot.damageBar2 / 600;
          break;
      }
      slot.earnedXP = num6;
      switch (roomType)
      {
        case RoomType.Destroy:
          using (PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_ACK generatorInfoAck = new PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_ACK(room))
            room.SendPacketToPlayers((SendPacket) generatorInfoAck, SlotState.BATTLE, 0);
          if (room.Bar1 == 0)
          {
            RoomSabotageSync.EndRound(room, (byte) 1);
            break;
          }
          if (room.Bar2 != 0)
            break;
          RoomSabotageSync.EndRound(room, (byte) 0);
          break;
        case RoomType.Defense:
          using (PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_ACK missionDefenceInfoAck = new PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_ACK(room))
            room.SendPacketToPlayers((SendPacket) missionDefenceInfoAck, SlotState.BATTLE, 0);
          if (room.Bar1 != 0 || room.Bar2 != 0)
            break;
          RoomSabotageSync.EndRound(room, (byte) 0);
          break;
      }
    }

    public static void EndRound(PointBlank.Game.Data.Model.Room room, byte winner)
    {
      room.swapRound = true;
      switch (winner)
      {
        case 0:
          ++room.red_rounds;
          break;
        case 1:
          ++room.blue_rounds;
          break;
      }
      AllUtils.BattleEndRound(room, (int) winner, RoundEndType.Normal);
    }
  }
}
