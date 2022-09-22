using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_CHANGE_ROOMINFO_REQ : ReceivePacket
  {
    public PROTOCOL_ROOM_CHANGE_ROOMINFO_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      try
      {
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room == null || room._state != RoomState.Ready || room._leader != player._slotId)
          return;
        this.readD();
        room.name = this.readUnicode(46);
        room.mapId = (MapIdEnum) this.readC();
        room.rule = (int) this.readC();
        room.stage = this.readC();
        room.room_type = (RoomType) this.readC();
        int num1 = (int) this.readC();
        int num2 = (int) this.readC();
        room.initSlotCount((int) this.readC(), true);
        room._ping = (int) this.readC();
        RoomWeaponsFlag roomWeaponsFlag = (RoomWeaponsFlag) this.readH();
        room.Flag = (RoomStageFlag) this.readD();
        int num3 = (int) this.readC();
        int num4 = (int) this.readC();
        int num5 = (int) this.readC();
        this.readB(66);
        room.killtime = (int) this.readC();
        int num6 = (int) this.readC();
        int num7 = (int) this.readC();
        int num8 = (int) this.readC();
        room.Limit = this.readC();
        room.WatchRuleFlag = this.readC();
        room.BalanceType = this.readH();
        this.readB(16);
        this.readB(4);
        room.aiCount = this.readC();
        room.aiLevel = this.readC();
        if (roomWeaponsFlag != room.weaponsFlag)
        {
          if (room.SniperMode)
            room.weaponsFlag = roomWeaponsFlag + 32;
          if (room.ShotgunMode)
            room.weaponsFlag = roomWeaponsFlag + 64;
          if (!room.ShotgunMode && !room.SniperMode)
            room.weaponsFlag = roomWeaponsFlag;
          for (int index = 0; index < 16; ++index)
          {
            Slot slot = room._slots[index];
            if (slot.state == SlotState.READY)
              slot.state = SlotState.NORMAL;
          }
        }
        room.SetSeed();
        room.updateRoomInfo();
        using (PROTOCOL_ROOM_CHANGE_ROOM_OPTIONINFO_ACK roomOptioninfoAck = new PROTOCOL_ROOM_CHANGE_ROOM_OPTIONINFO_ACK(room, room.getLeader().player_name))
          room.SendPacketToPlayers((SendPacket) roomOptioninfoAck);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BATTLE_CHANGE_ROOMINFO_REQ: " + ex.ToString());
      }
    }

    public override void run()
    {
    }
  }
}
