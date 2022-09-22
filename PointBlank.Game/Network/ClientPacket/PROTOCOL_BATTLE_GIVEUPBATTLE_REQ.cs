using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_GIVEUPBATTLE_REQ : ReceivePacket
  {
    private bool isFinished;
    private long objId;

    public PROTOCOL_BATTLE_GIVEUPBATTLE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.objId = (long) this.readD();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        PointBlank.Core.Models.Room.Slot slot1;
        if (room == null || room._state < RoomState.Loading || (!room.getSlot(player._slotId, out slot1) || slot1.state < SlotState.LOAD))
          return;
        bool isBotMode = room.isBotMode();
        this.FreepassEffect(player, slot1, room, isBotMode);
        if (room.vote.Timer != null && room.votekick != null && room.votekick.victimIdx == slot1._id)
        {
          room.vote.Timer = (Timer) null;
          room.votekick = (PointBlank.Core.Models.Room.VoteKick) null;
          using (PROTOCOL_BATTLE_NOTIFY_KICKVOTE_CANCEL_ACK kickvoteCancelAck = new PROTOCOL_BATTLE_NOTIFY_KICKVOTE_CANCEL_ACK())
            room.SendPacketToPlayers((SendPacket) kickvoteCancelAck, SlotState.BATTLE, 0, slot1._id);
        }
        AllUtils.ResetSlotInfo(room, slot1, true);
        int red13 = 0;
        int blue13 = 0;
        int num1 = 0;
        int num2 = 0;
        foreach (PointBlank.Core.Models.Room.Slot slot2 in room._slots)
        {
          if (slot2.state >= SlotState.LOAD)
          {
            if (slot2._team == 0)
              ++num1;
            else
              ++num2;
            if (slot2.state == SlotState.BATTLE)
            {
              if (slot2._team == 0)
                ++red13;
              else
                ++blue13;
            }
          }
        }
        if (slot1._id == room._leader)
        {
          if (isBotMode)
          {
            if (red13 > 0 || blue13 > 0)
              this.LeaveHostBOT_GiveBattle(room, player);
            else
              this.LeaveHostBOT_EndBattle(room, player);
          }
          else if (room._state == RoomState.Battle && (red13 == 0 || blue13 == 0) || room._state <= RoomState.PreBattle && (num1 == 0 || num2 == 0))
            this.LeaveHostNoBOT_EndBattle(room, player, red13, blue13);
          else
            this.LeaveHostNoBOT_GiveBattle(room, player);
        }
        else if (!isBotMode)
        {
          if (room._state == RoomState.Battle && (red13 == 0 || blue13 == 0) || room._state <= RoomState.PreBattle && (num1 == 0 || num2 == 0))
            this.LeavePlayerNoBOT_EndBattle(room, player, red13, blue13);
          else
            this.LeavePlayer_QuitBattle(room, player);
        }
        else
          this.LeavePlayer_QuitBattle(room, player);
        this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(player, 0));
        if (this.isFinished || room._state != RoomState.Battle)
          return;
        AllUtils.BattleEndRoundPlayersCount(room);
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_BATTLE_GIVEUPBATTLE_REQ: " + ex.ToString());
      }
    }

    private void FreepassEffect(Account p, PointBlank.Core.Models.Room.Slot slot, PointBlank.Game.Data.Model.Room room, bool isBotMode)
    {
      DBQuery dbQuery = new DBQuery();
      if (p._bonus.freepass == 0 || p._bonus.freepass == 1 && room._channelType == 4)
      {
        if (isBotMode || slot.state < SlotState.BATTLE_READY)
          return;
        if (p._gp > 0)
        {
          p._gp -= 200;
          if (p._gp < 0)
            p._gp = 0;
          dbQuery.AddQuery("gp", (object) p._gp);
        }
        dbQuery.AddQuery("escapes", (object) ++p._statistic.escapes);
      }
      else
      {
        if (room._state != RoomState.Battle)
          return;
        int num1 = 0;
        int num2 = 0;
        int num3;
        int num4;
        if (isBotMode)
        {
          int num5 = (int) room.IngameAiLevel * (150 + slot.allDeaths);
          if (num5 == 0)
            ++num5;
          int num6 = slot.Score / num5;
          num3 = num2 + num6;
          num4 = num1 + num6;
        }
        else
        {
          int num5 = slot.allKills != 0 || slot.allDeaths != 0 ? (int) slot.inBattleTime(DateTime.Now) : 0;
          if (room.room_type == RoomType.Bomb || room.room_type == RoomType.FreeForAll || room.room_type == RoomType.Convoy)
          {
            num4 = (int) ((double) slot.Score + (double) num5 / 2.5 + (double) slot.allDeaths * 2.2 + (double) (slot.objects * 20));
            num3 = (int) ((double) slot.Score + (double) num5 / 3.0 + (double) slot.allDeaths * 2.2 + (double) (slot.objects * 20));
          }
          else
          {
            num4 = (int) ((double) slot.Score + (double) num5 / 2.5 + (double) slot.allDeaths * 1.8 + (double) (slot.objects * 20));
            num3 = (int) ((double) slot.Score + (double) num5 / 3.0 + (double) slot.allDeaths * 1.8 + (double) (slot.objects * 20));
          }
        }
        p._exp += GameConfig.maxBattleXP < num4 ? GameConfig.maxBattleXP : num4;
        p._gp += GameConfig.maxBattleGP < num3 ? GameConfig.maxBattleGP : num3;
        if (num3 > 0)
          dbQuery.AddQuery("gp", (object) p._gp);
        if (num4 > 0)
          dbQuery.AddQuery("exp", (object) p._exp);
      }
      ComDiv.updateDB("accounts", "player_id", (object) p.player_id, dbQuery.GetTables(), dbQuery.GetValues());
    }

    private void LeaveHostBOT_GiveBattle(PointBlank.Game.Data.Model.Room room, Account p)
    {
      List<Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
      if (allPlayers.Count == 0)
        return;
      int leader = room._leader;
      room.setNewLeader(-1, 14, room._leader, true);
      using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK battleGiveupbattleAck = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(p, 0))
      {
        using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK leaveP2PserverAck = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(room))
        {
          byte[] completeBytes1 = battleGiveupbattleAck.GetCompleteBytes("PROTOCOL_BATTLE_GIVEUPBATTLE_REQ-1");
          byte[] completeBytes2 = leaveP2PserverAck.GetCompleteBytes("PROTOCOL_BATTLE_GIVEUPBATTLE_REQ-2");
          foreach (Account account in allPlayers)
          {
            PointBlank.Core.Models.Room.Slot slot = room.getSlot(account._slotId);
            if (slot != null)
            {
              if (slot.state >= SlotState.PRESTART)
                account.SendCompletePacket(completeBytes2);
              account.SendCompletePacket(completeBytes1);
            }
          }
        }
      }
    }

    private void LeaveHostBOT_EndBattle(PointBlank.Game.Data.Model.Room room, Account p)
    {
      List<Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
      if (allPlayers.Count != 0)
      {
        using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK battleGiveupbattleAck = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(p, 0))
        {
          byte[] completeBytes = battleGiveupbattleAck.GetCompleteBytes("PROTOCOL_BATTLE_GIVEUPBATTLE_REQ-3");
          TeamResultType winnerTeam = AllUtils.GetWinnerTeam(room);
          ushort MissionFlag;
          ushort SlotFlag;
          AllUtils.getBattleResult(room, out MissionFlag, out SlotFlag);
          foreach (Account p1 in allPlayers)
          {
            p1.SendCompletePacket(completeBytes);
            p1.SendPacket((SendPacket) new PROTOCOL_BATTLE_ENDBATTLE_ACK(p1, winnerTeam, SlotFlag, MissionFlag, true));
          }
        }
      }
      AllUtils.resetBattleInfo(room);
    }

    private void LeaveHostNoBOT_EndBattle(PointBlank.Game.Data.Model.Room room, Account p, int red13, int blue13)
    {
      this.isFinished = true;
      List<Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
      if (allPlayers.Count != 0)
      {
        TeamResultType winnerTeam = AllUtils.GetWinnerTeam(room, red13, blue13);
        if (room._state == RoomState.Battle)
          room.CalculateResult(winnerTeam, false);
        using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK battleGiveupbattleAck = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(p, 0))
        {
          byte[] completeBytes = battleGiveupbattleAck.GetCompleteBytes("PROTOCOL_BATTLE_GIVEUPBATTLE_REQ-4");
          ushort MissionFlag;
          ushort SlotFlag;
          AllUtils.getBattleResult(room, out MissionFlag, out SlotFlag);
          foreach (Account p1 in allPlayers)
          {
            p1.SendCompletePacket(completeBytes);
            p1.SendPacket((SendPacket) new PROTOCOL_BATTLE_ENDBATTLE_ACK(p1, winnerTeam, SlotFlag, MissionFlag, false));
          }
        }
      }
      AllUtils.resetBattleInfo(room);
    }

    private void LeaveHostNoBOT_GiveBattle(PointBlank.Game.Data.Model.Room room, Account p)
    {
      List<Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
      if (allPlayers.Count == 0)
        return;
      int leader = room._leader;
      int state = room._state == RoomState.Battle ? 14 : 9;
      room.setNewLeader(-1, state, room._leader, true);
      using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK leaveP2PserverAck = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(room))
      {
        using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK battleGiveupbattleAck = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(p, 0))
        {
          byte[] completeBytes1 = leaveP2PserverAck.GetCompleteBytes("PROTOCOL_BATTLE_GIVEUPBATTLE_REQ-6");
          byte[] completeBytes2 = battleGiveupbattleAck.GetCompleteBytes("PROTOCOL_BATTLE_GIVEUPBATTLE_REQ-7");
          foreach (Account account in allPlayers)
          {
            if (room._slots[account._slotId].state >= SlotState.PRESTART)
              account.SendCompletePacket(completeBytes1);
            account.SendCompletePacket(completeBytes2);
          }
        }
      }
    }

    private void LeavePlayerNoBOT_EndBattle(PointBlank.Game.Data.Model.Room room, Account p, int red13, int blue13)
    {
      this.isFinished = true;
      TeamResultType winnerTeam = AllUtils.GetWinnerTeam(room, red13, blue13);
      List<Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
      if (allPlayers.Count != 0)
      {
        if (room._state == RoomState.Battle)
          room.CalculateResult(winnerTeam, false);
        using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK battleGiveupbattleAck = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(p, 0))
        {
          byte[] completeBytes = battleGiveupbattleAck.GetCompleteBytes("PROTOCOL_BATTLE_GIVEUPBATTLE_REQ-8");
          ushort MissionFlag;
          ushort SlotFlag;
          AllUtils.getBattleResult(room, out MissionFlag, out SlotFlag);
          foreach (Account p1 in allPlayers)
          {
            p1.SendCompletePacket(completeBytes);
            p1.SendPacket((SendPacket) new PROTOCOL_BATTLE_ENDBATTLE_ACK(p1, winnerTeam, SlotFlag, MissionFlag, false));
          }
        }
      }
      AllUtils.resetBattleInfo(room);
    }

    private void LeavePlayer_QuitBattle(PointBlank.Game.Data.Model.Room room, Account p)
    {
      using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK battleGiveupbattleAck = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(p, 0))
        room.SendPacketToPlayers((SendPacket) battleGiveupbattleAck, SlotState.READY, 1);
    }
  }
}
