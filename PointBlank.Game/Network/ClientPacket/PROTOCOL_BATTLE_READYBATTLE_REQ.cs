using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Map;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_READYBATTLE_REQ : ReceivePacket
  {
    private int Error;

    public PROTOCOL_BATTLE_READYBATTLE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      int num = (int) this.readC();
      this.Error = this.readD();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null)
          return;
        PointBlank.Game.Data.Model.Room room = player._room;
        Channel ch;
        Slot slot;
        if (room == null || room.getLeader() == null || (!room.getChannel(out ch) || !room.getSlot(player._slotId, out slot)))
          return;
        if (slot._equip == null)
        {
          this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_READYBATTLE_ACK(2147487915U));
        }
        else
        {
          bool isBotMode = room.isBotMode();
          slot.specGM = this.Error == 1 && player.IsGM();
          player.DebugPing = false;
          if (room._leader == player._slotId)
          {
            if (room._state != RoomState.Ready && room._state != RoomState.CountDown)
              return;
            int TotalEnemys = 0;
            int redPlayers = 0;
            int bluePlayers = 0;
            this.GetReadyPlayers(room, ref redPlayers, ref bluePlayers, ref TotalEnemys);
            if (GameConfig.isTestMode && GameConfig.udpType == UdpState.RELAY)
              TotalEnemys = 1;
            int num = 0;
            MapMatch mapMatch = MapModel.Matchs.Find((Predicate<MapMatch>) (x =>
            {
              if ((MapIdEnum) x.Id == room.mapId)
                return MapModel.getRule(x.Mode).Rule == room.rule;
              return false;
            }));
            if (mapMatch != null)
              num = mapMatch.Limit;
            if (num == 8 && (redPlayers >= 4 || bluePlayers >= 4) && ch._type != 4)
            {
              this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_UNREADY_4VS4_ACK());
            }
            else
            {
              if (this.ClanMatchCheck(room, ch._type, TotalEnemys))
                return;
              if (isBotMode || room.room_type == RoomType.Tutorial || TotalEnemys > 0 && !isBotMode)
              {
                room.changeSlotState(slot, SlotState.READY, false);
                if (room._state != RoomState.CountDown)
                  this.TryBalanceTeams(room, isBotMode);
                if (room.thisModeHaveCD())
                {
                  if (room._state == RoomState.Ready)
                  {
                    room._state = RoomState.CountDown;
                    room.updateRoomInfo();
                    room.StartCountDown();
                  }
                  else if (room._state == RoomState.CountDown)
                  {
                    room.changeSlotState(room._leader, SlotState.NORMAL, false);
                    room.StopCountDown(CountDownEnum.StopByHost, true);
                  }
                }
                else
                  room.StartBattle(false);
                room.updateSlotsInfo();
              }
              else
              {
                if (TotalEnemys != 0 || isBotMode)
                  return;
                this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_READYBATTLE_ACK(2147487753U));
              }
            }
          }
          else if (room._slots[room._leader].state >= SlotState.LOAD)
          {
            if (slot.state != SlotState.NORMAL)
              return;
            if (room.BalanceType == (short) 1 && !isBotMode)
              AllUtils.TryBalancePlayer(room, player, true, ref slot);
            room.changeSlotState(slot, SlotState.LOAD, true);
            slot.SetMissionsClone(player._mission);
            this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_READYBATTLE_ACK((uint) slot.state));
            this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_START_GAME_ACK(room));
            using (PROTOCOL_BATTLE_START_GAME_TRANS_ACK startGameTransAck = new PROTOCOL_BATTLE_START_GAME_TRANS_ACK(room, slot, player._titles))
              room.SendPacketToPlayers((SendPacket) startGameTransAck, SlotState.READY, 1, slot._id);
          }
          else if (slot.state == SlotState.NORMAL)
          {
            room.changeSlotState(slot, SlotState.READY, true);
            if (room._state != RoomState.CountDown)
              return;
            this.TryBalanceTeams(room, isBotMode);
          }
          else
          {
            if (slot.state != SlotState.READY)
              return;
            room.changeSlotState(slot, SlotState.NORMAL, false);
            if (room._state == RoomState.CountDown && room.getPlayingPlayers(room._leader % 2 == 0 ? 1 : 0, SlotState.READY, 0) == 0)
            {
              room.changeSlotState(room._leader, SlotState.NORMAL, false);
              room.StopCountDown(CountDownEnum.StopByPlayer, true);
            }
            room.updateSlotsInfo();
          }
        }
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BATTLE_READYBATTLE_REQ: " + ex.ToString());
      }
    }

    private void GetReadyPlayers(
      PointBlank.Game.Data.Model.Room room,
      ref int redPlayers,
      ref int bluePlayers,
      ref int TotalEnemys)
    {
      for (int index = 0; index < 16; ++index)
      {
        Slot slot = room._slots[index];
        if (slot.state == SlotState.READY)
        {
          if (slot._team == 0)
            ++redPlayers;
          else
            ++bluePlayers;
        }
      }
      if (room._leader % 2 == 0)
        TotalEnemys = bluePlayers;
      else
        TotalEnemys = redPlayers;
    }

    private bool ClanMatchCheck(PointBlank.Game.Data.Model.Room room, int type, int TotalEnemys)
    {
      if (GameConfig.isTestMode || type != 4)
        return false;
      if (!AllUtils.Have2ClansToClanMatch(room))
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_READYBATTLE_ACK(2147487857U));
        return true;
      }
      if (TotalEnemys <= 0 || AllUtils.HavePlayersToClanMatch(room))
        return false;
      this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_READYBATTLE_ACK(2147487858U));
      return true;
    }

    private void TryBalanceTeams(PointBlank.Game.Data.Model.Room room, bool isBotMode)
    {
      if (room.BalanceType != (short) 1 | isBotMode)
        return;
      int[] numArray1;
      switch (AllUtils.getBalanceTeamIdx(room, false, -1))
      {
        case -1:
          return;
        case 1:
          numArray1 = room.RED_TEAM;
          break;
        default:
          numArray1 = room.BLUE_TEAM;
          break;
      }
      int[] numArray2 = numArray1;
      Slot mySlot = (Slot) null;
      for (int index = numArray2.Length - 1; index >= 0; --index)
      {
        Slot slot = room._slots[numArray2[index]];
        if (slot.state == SlotState.READY && room._leader != slot._id)
        {
          mySlot = slot;
          break;
        }
      }
      Account player;
      if (mySlot == null || !room.getPlayerBySlot(mySlot, out player))
        return;
      AllUtils.TryBalancePlayer(room, player, false, ref mySlot);
    }
  }
}
