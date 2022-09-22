using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_PRESTARTBATTLE_REQ : ReceivePacket
  {
    private int stage;
    private int rule;
    private MapIdEnum mapId;
    private RoomType room_type;

    public PROTOCOL_BATTLE_PRESTARTBATTLE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.mapId = (MapIdEnum) this.readC();
      this.rule = (int) this.readC();
      this.stage = (int) this.readC();
      this.room_type = (RoomType) this.readC();
    }

    public override void run()
    {
      try
      {
        if (this._client == null)
          return;
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room != null && (int) room.stage == this.stage && (room.room_type == this.room_type && room.mapId == this.mapId) && room.rule == this.rule)
        {
          Slot slot = room._slots[player._slotId];
          if (room.isPreparing() && room.UdpServer != null && slot.state >= SlotState.LOAD)
          {
            Account leader = room.getLeader();
            if (leader != null)
            {
              if (string.IsNullOrEmpty(player.PublicIP.ToString()))
              {
                this._client.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_ACK(EventErrorEnum.BATTLE_NO_REAL_IP));
                this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(player, 0));
                room.changeSlotState(slot, SlotState.NORMAL, true);
                AllUtils.BattleEndPlayersCount(room, room.isBotMode());
                slot.StopTiming();
              }
              else
              {
                if (slot._id == room._leader)
                {
                  room._state = RoomState.PreBattle;
                  room.updateRoomInfo();
                }
                slot.preStartDate = DateTime.Now;
                room.StartCounter(1, player, slot);
                room.changeSlotState(slot, SlotState.PRESTART, true);
                this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_PRESTARTBATTLE_ACK(player, leader, true));
                if (slot._id == room._leader)
                  return;
                leader.SendPacket((SendPacket) new PROTOCOL_BATTLE_PRESTARTBATTLE_ACK(player, leader, false));
              }
            }
            else
            {
              this._client.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_ACK(EventErrorEnum.BATTLE_FIRST_HOLE));
              this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(player, 0));
              room.changeSlotState(slot, SlotState.NORMAL, true);
              AllUtils.BattleEndPlayersCount(room, room.isBotMode());
              slot.StopTiming();
            }
          }
          else
          {
            room.changeSlotState(slot, SlotState.NORMAL, true);
            this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_STARTBATTLE_ACK());
            AllUtils.BattleEndPlayersCount(room, room.isBotMode());
            slot.StopTiming();
          }
        }
        else
        {
          this._client.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_ACK(EventErrorEnum.BATTLE_FIRST_MAINLOAD));
          this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_PRESTARTBATTLE_ACK());
          if (room != null)
          {
            room.changeSlotState(player._slotId, SlotState.NORMAL, true);
            AllUtils.BattleEndPlayersCount(room, room.isBotMode());
          }
          else
            this._client.SendPacket((SendPacket) new PROTOCOL_LOBBY_ENTER_ACK());
        }
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BATTLE_PRESTARTBATTLE_REQ: " + ex.ToString());
      }
    }
  }
}
