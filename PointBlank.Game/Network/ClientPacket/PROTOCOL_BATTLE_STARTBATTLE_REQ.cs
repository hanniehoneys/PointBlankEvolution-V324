using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_STARTBATTLE_REQ : ReceivePacket
  {
    public PROTOCOL_BATTLE_STARTBATTLE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      try
      {
        if (this._client == null)
          return;
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room != null && room.isPreparing())
        {
          bool isBotMode = room.isBotMode();
          Slot slot1 = room.getSlot(player._slotId);
          if (slot1 == null)
            return;
          if (slot1.state == SlotState.PRESTART)
          {
            room.changeSlotState(slot1, SlotState.BATTLE_READY, true);
            slot1.StopTiming();
            if (isBotMode)
              this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK(room));
            this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_CHANGE_ROOMINFO_ACK(room, isBotMode));
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            for (int index = 0; index < 16; ++index)
            {
              Slot slot2 = room._slots[index];
              if (slot2.state >= SlotState.LOAD)
              {
                ++num3;
                if (slot2._team == 0)
                  ++num4;
                else
                  ++num5;
                if (slot2.state >= SlotState.BATTLE_READY)
                {
                  if (slot2._team == 0)
                    ++num2;
                  else
                    ++num1;
                }
              }
            }
            if (room._state != RoomState.Battle && (!(room._slots[room._leader].state >= SlotState.BATTLE_READY & isBotMode) || (room._leader % 2 != 0 || num2 <= num4 / 2) && (room._leader % 2 != 1 || num1 <= num5 / 2)) && (room._slots[room._leader].state < SlotState.BATTLE_READY || (GameConfig.isTestMode && GameConfig.udpType == UdpState.RELAY || (num1 <= num5 / 2 || num2 <= num4 / 2)) && (!GameConfig.isTestMode || GameConfig.udpType != UdpState.RELAY)))
              return;
            room.SpawnReadyPlayers(isBotMode);
          }
          else
          {
            this._client.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_ACK(EventErrorEnum.BATTLE_FIRST_HOLE));
            this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(player, 0));
            room.changeSlotState(slot1, SlotState.NORMAL, true);
            AllUtils.BattleEndPlayersCount(room, isBotMode);
          }
        }
        else
        {
          this._client.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_ACK(EventErrorEnum.BATTLE_FIRST_HOLE));
          this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_STARTBATTLE_ACK());
          room?.changeSlotState(player._slotId, SlotState.NORMAL, true);
          if (room != null || player == null)
            return;
          this._client.SendPacket((SendPacket) new PROTOCOL_LOBBY_ENTER_ACK());
        }
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_BATTLE_STARTBATTLE_REQ: " + ex.ToString());
      }
    }
  }
}
