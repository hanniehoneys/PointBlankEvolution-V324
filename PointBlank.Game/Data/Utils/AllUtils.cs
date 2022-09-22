using Npgsql;
using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Account.Mission;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Core.Sql;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Sync;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace PointBlank.Game.Data.Utils
{
  public static class AllUtils
  {
    public static int getKillScore(KillingMessage msg)
    {
      int num = 0;
      switch (msg)
      {
        case KillingMessage.PiercingShot:
        case KillingMessage.MassKill:
          num += 6;
          goto case KillingMessage.Suicide;
        case KillingMessage.ChainStopper:
          num += 8;
          goto case KillingMessage.Suicide;
        case KillingMessage.Headshot:
          num += 10;
          goto case KillingMessage.Suicide;
        case KillingMessage.ChainHeadshot:
          num += 14;
          goto case KillingMessage.Suicide;
        case KillingMessage.ChainSlugger:
          num += 6;
          goto case KillingMessage.Suicide;
        case KillingMessage.Suicide:
          return num;
        case KillingMessage.ObjectDefense:
          num += 7;
          goto case KillingMessage.Suicide;
        default:
          num += 5;
          goto case KillingMessage.Suicide;
      }
    }

    public static void CompleteMission(
      PointBlank.Game.Data.Model.Room room,
      PointBlank.Core.Models.Room.Slot slot,
      FragInfos kills,
      MissionType autoComplete,
      int moreInfo)
    {
      try
      {
        PointBlank.Game.Data.Model.Account playerBySlot = room.getPlayerBySlot(slot);
        if (playerBySlot == null)
          return;
        AllUtils.MissionCompleteBase(room, playerBySlot, slot, kills, autoComplete, moreInfo);
      }
      catch (Exception ex)
      {
        Logger.error("[AllUtils.CompleteMission1] " + ex.ToString());
      }
    }

    public static void CompleteMission(
      PointBlank.Game.Data.Model.Room room,
      PointBlank.Core.Models.Room.Slot slot,
      MissionType autoComplete,
      int moreInfo)
    {
      try
      {
        PointBlank.Game.Data.Model.Account playerBySlot = room.getPlayerBySlot(slot);
        if (playerBySlot == null)
          return;
        AllUtils.MissionCompleteBase(room, playerBySlot, slot, autoComplete, moreInfo);
      }
      catch (Exception ex)
      {
        Logger.error("[AllUtils.CompleteMission2] " + ex.ToString());
      }
    }

    public static void CompleteMission(
      PointBlank.Game.Data.Model.Room room,
      PointBlank.Game.Data.Model.Account player,
      PointBlank.Core.Models.Room.Slot slot,
      FragInfos kills,
      MissionType autoComplete,
      int moreInfo)
    {
      AllUtils.MissionCompleteBase(room, player, slot, kills, autoComplete, moreInfo);
    }

    public static void CompleteMission(
      PointBlank.Game.Data.Model.Room room,
      PointBlank.Game.Data.Model.Account player,
      PointBlank.Core.Models.Room.Slot slot,
      MissionType autoComplete,
      int moreInfo)
    {
      AllUtils.MissionCompleteBase(room, player, slot, autoComplete, moreInfo);
    }

    private static void MissionCompleteBase(
      PointBlank.Game.Data.Model.Room room,
      PointBlank.Game.Data.Model.Account pR,
      PointBlank.Core.Models.Room.Slot slot,
      FragInfos kills,
      MissionType autoComplete,
      int moreInfo)
    {
      try
      {
        PlayerMissions missions = slot.Missions;
        if (missions == null)
          return;
        int currentMissionId = missions.getCurrentMissionId();
        int currentCard = missions.getCurrentCard();
        if (currentMissionId <= 0 || missions.selectedCard)
          return;
        List<Card> cards = MissionCardXml.getCards(currentMissionId, currentCard);
        if (cards.Count == 0)
          return;
        KillingMessage allKillFlags = kills.GetAllKillFlags();
        byte[] currentMissionList = missions.getCurrentMissionList();
        ClassType idClassType = ComDiv.getIdClassType(kills.weapon);
        ClassType convertedClass1 = AllUtils.ConvertWeaponClass(idClassType);
        int idStatics = ComDiv.getIdStatics(kills.weapon, 3);
        ClassType weaponClass = moreInfo > 0 ? ComDiv.getIdClassType(moreInfo) : ClassType.Unknown;
        ClassType convertedClass2 = moreInfo > 0 ? AllUtils.ConvertWeaponClass(weaponClass) : ClassType.Unknown;
        int weaponId = moreInfo > 0 ? ComDiv.getIdStatics(moreInfo, 3) : 0;
        foreach (Card card in cards)
        {
          int num = 0;
          if (card._mapId == 0 || (MapIdEnum) card._mapId == room.mapId)
          {
            if (kills.frags.Count > 0)
            {
              if (card._missionType == MissionType.KILL || card._missionType == MissionType.CHAINSTOPPER && allKillFlags.HasFlag((Enum) KillingMessage.ChainStopper) || (card._missionType == MissionType.CHAINSLUGGER && allKillFlags.HasFlag((Enum) KillingMessage.ChainSlugger) || card._missionType == MissionType.CHAINKILLER && slot.killsOnLife >= 4) || (card._missionType == MissionType.TRIPLE_KILL && slot.killsOnLife == 3 || card._missionType == MissionType.DOUBLE_KILL && slot.killsOnLife == 2 || card._missionType == MissionType.HEADSHOT && (allKillFlags.HasFlag((Enum) KillingMessage.Headshot) || allKillFlags.HasFlag((Enum) KillingMessage.ChainHeadshot))) || (card._missionType == MissionType.CHAINHEADSHOT && allKillFlags.HasFlag((Enum) KillingMessage.ChainHeadshot) || card._missionType == MissionType.PIERCING && allKillFlags.HasFlag((Enum) KillingMessage.PiercingShot) || card._missionType == MissionType.MASS_KILL && allKillFlags.HasFlag((Enum) KillingMessage.MassKill) || card._missionType == MissionType.KILL_MAN && (room.room_type == RoomType.Boss || room.room_type == RoomType.CrossCounter) && (slot._team == 1 && room.rounds == 2 || slot._team == 0 && room.rounds == 1)))
                num = AllUtils.CheckPlayersClass1(card, idClassType, convertedClass1, idStatics, kills);
              else if (card._missionType == MissionType.KILL_WEAPONCLASS || card._missionType == MissionType.DOUBLE_KILL_WEAPONCLASS && slot.killsOnLife == 2 || card._missionType == MissionType.TRIPLE_KILL_WEAPONCLASS && slot.killsOnLife == 3)
                num = AllUtils.CheckPlayersClass2(card, kills);
            }
            else if (card._missionType == MissionType.DEATHBLOW && autoComplete == MissionType.DEATHBLOW)
              num = AllUtils.CheckPlayerClass(card, weaponClass, convertedClass2, weaponId);
            else if (card._missionType == autoComplete)
              num = 1;
          }
          if (num != 0)
          {
            int arrayIdx = card._arrayIdx;
            if ((int) currentMissionList[arrayIdx] + 1 <= card._missionLimit)
            {
              slot.MissionsCompleted = true;
              currentMissionList[arrayIdx] += (byte) num;
              if ((int) currentMissionList[arrayIdx] > card._missionLimit)
                currentMissionList[arrayIdx] = (byte) card._missionLimit;
              int progress = (int) currentMissionList[arrayIdx];
              pR.SendPacket((SendPacket) new PROTOCOL_BASE_QUEST_CHANGE_ACK(progress, card));
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    private static void MissionCompleteBase(
      PointBlank.Game.Data.Model.Room room,
      PointBlank.Game.Data.Model.Account pR,
      PointBlank.Core.Models.Room.Slot slot,
      MissionType autoComplete,
      int moreInfo)
    {
      try
      {
        PlayerMissions missions = slot.Missions;
        if (missions == null)
          return;
        int currentMissionId = missions.getCurrentMissionId();
        int currentCard = missions.getCurrentCard();
        if (currentMissionId <= 0 || missions.selectedCard)
          return;
        List<Card> cards = MissionCardXml.getCards(currentMissionId, currentCard);
        if (cards.Count == 0)
          return;
        byte[] currentMissionList = missions.getCurrentMissionList();
        ClassType weaponClass = moreInfo > 0 ? ComDiv.getIdClassType(moreInfo) : ClassType.Unknown;
        ClassType convertedClass = moreInfo > 0 ? AllUtils.ConvertWeaponClass(weaponClass) : ClassType.Unknown;
        int weaponId = moreInfo > 0 ? ComDiv.getIdStatics(moreInfo, 3) : 0;
        foreach (Card card in cards)
        {
          int num = 0;
          if (card._mapId == 0 || (MapIdEnum) card._mapId == room.mapId)
          {
            if (card._missionType == MissionType.DEATHBLOW && autoComplete == MissionType.DEATHBLOW)
              num = AllUtils.CheckPlayerClass(card, weaponClass, convertedClass, weaponId);
            else if (card._missionType == autoComplete)
              num = 1;
          }
          if (num != 0)
          {
            int arrayIdx = card._arrayIdx;
            if ((int) currentMissionList[arrayIdx] + 1 <= card._missionLimit)
            {
              slot.MissionsCompleted = true;
              currentMissionList[arrayIdx] += (byte) num;
              if ((int) currentMissionList[arrayIdx] > card._missionLimit)
                currentMissionList[arrayIdx] = (byte) card._missionLimit;
              int progress = (int) currentMissionList[arrayIdx];
              pR.SendPacket((SendPacket) new PROTOCOL_BASE_QUEST_CHANGE_ACK(progress, card));
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    private static int CheckPlayersClass1(
      Card card,
      ClassType weaponClass,
      ClassType convertedClass,
      int weaponId,
      FragInfos infos)
    {
      int num = 0;
      if ((card._weaponReqId == 0 || card._weaponReqId == weaponId) && (card._weaponReq == ClassType.Unknown || card._weaponReq == weaponClass || card._weaponReq == convertedClass))
      {
        foreach (Frag frag in infos.frags)
        {
          if (frag.VictimSlot % 2 != (int) infos.killerIdx % 2)
            ++num;
        }
      }
      return num;
    }

    private static int CheckPlayersClass2(Card card, FragInfos infos)
    {
      int num = 0;
      foreach (Frag frag in infos.frags)
      {
        if (frag.VictimSlot % 2 != (int) infos.killerIdx % 2 && (card._weaponReq == ClassType.Unknown || card._weaponReq == (ClassType) frag.victimWeaponClass || card._weaponReq == AllUtils.ConvertWeaponClass((ClassType) frag.victimWeaponClass)))
          ++num;
      }
      return num;
    }

    private static int CheckPlayerClass(
      Card card,
      ClassType weaponClass,
      ClassType convertedClass,
      int weaponId,
      int killerId,
      Frag frag)
    {
      return (card._weaponReqId == 0 || card._weaponReqId == weaponId) && (card._weaponReq == ClassType.Unknown || card._weaponReq == weaponClass || card._weaponReq == convertedClass) && frag.VictimSlot % 2 != killerId % 2 ? 1 : 0;
    }

    private static int CheckPlayerClass(
      Card card,
      ClassType weaponClass,
      ClassType convertedClass,
      int weaponId)
    {
      return (card._weaponReqId == 0 || card._weaponReqId == weaponId) && (card._weaponReq == ClassType.Unknown || card._weaponReq == weaponClass || card._weaponReq == convertedClass) ? 1 : 0;
    }

    private static ClassType ConvertWeaponClass(ClassType weaponClass)
    {
      switch (weaponClass)
      {
        case ClassType.DualHandGun:
          return ClassType.HandGun;
        case ClassType.DualKnife:
        case ClassType.Knuckle:
          return ClassType.Knife;
        case ClassType.DualSMG:
          return ClassType.SMG;
        case ClassType.DualShotgun:
          return ClassType.Shotgun;
        default:
          return weaponClass;
      }
    }

    public static TeamResultType GetWinnerTeam(PointBlank.Game.Data.Model.Room room)
    {
      if (room == null)
        return TeamResultType.TeamDraw;
      byte num = 0;
      if (room.room_type == RoomType.Bomb || room.room_type == RoomType.Destroy || (room.room_type == RoomType.Annihilation || room.room_type == RoomType.Defense) || room.room_type == RoomType.Convoy)
      {
        if (room.blue_rounds == room.red_rounds)
          num = (byte) 2;
        else if (room.blue_rounds > room.red_rounds)
          num = (byte) 1;
        else if (room.blue_rounds < room.red_rounds)
          num = (byte) 0;
      }
      else if (room.room_type == RoomType.Boss)
      {
        if (room.blue_dino == room.red_dino)
          num = (byte) 2;
        else if (room.blue_dino > room.red_dino)
          num = (byte) 1;
        else if (room.blue_dino < room.red_dino)
          num = (byte) 0;
      }
      else if (room._blueKills == room._redKills)
        num = (byte) 2;
      else if (room._blueKills > room._redKills)
        num = (byte) 1;
      else if (room._blueKills < room._redKills)
        num = (byte) 0;
      return (TeamResultType) num;
    }

    public static TeamResultType GetWinnerTeam(
      PointBlank.Game.Data.Model.Room room,
      int RedPlayers,
      int BluePlayers)
    {
      if (room == null)
        return TeamResultType.TeamDraw;
      byte num = 2;
      if (RedPlayers == 0)
        num = (byte) 1;
      else if (BluePlayers == 0)
        num = (byte) 0;
      return (TeamResultType) num;
    }

    public static void endMatchMission(
      PointBlank.Game.Data.Model.Room room,
      PointBlank.Game.Data.Model.Account player,
      PointBlank.Core.Models.Room.Slot slot,
      TeamResultType winnerTeam)
    {
      if (winnerTeam == TeamResultType.TeamDraw)
        return;
      AllUtils.CompleteMission(room, player, slot, (TeamResultType) slot._team == winnerTeam ? MissionType.WIN : MissionType.DEFEAT, 0);
    }

    public static void updateMatchCount(
      bool WonTheMatch,
      PointBlank.Game.Data.Model.Account p,
      int winnerTeam,
      DBQuery query)
    {
      if (winnerTeam == 2)
        query.AddQuery("fights_draw", (object) ++p._statistic.fights_draw);
      else if (WonTheMatch)
        query.AddQuery("fights_win", (object) ++p._statistic.fights_win);
      else
        query.AddQuery("fights_lost", (object) ++p._statistic.fights_lost);
      query.AddQuery("fights", (object) ++p._statistic.fights);
      query.AddQuery("totalfights_count", (object) ++p._statistic.totalfights_count);
    }

    public static void UpdateDailyRecord(
      bool WonTheMatch,
      PointBlank.Game.Data.Model.Account p,
      int winnerTeam,
      DBQuery query)
    {
      if (winnerTeam == 2)
        query.AddQuery("draws", (object) ++p.Daily.Draws);
      else if (WonTheMatch)
        query.AddQuery("wins", (object) ++p.Daily.Wins);
      else
        query.AddQuery("loses", (object) ++p.Daily.Loses);
      query.AddQuery("total", (object) ++p.Daily.Total);
    }

    public static void updateMatchCountFreeForAll(
      PointBlank.Game.Data.Model.Room Room,
      PointBlank.Game.Data.Model.Account p,
      int SlotWin,
      DBQuery query)
    {
      int[] numArray = new int[16];
      for (int index = 0; index < numArray.Length; ++index)
      {
        PointBlank.Core.Models.Room.Slot slot = Room._slots[index];
        numArray[index] = slot._playerId == 0L ? 0 : slot.allKills;
      }
      int index1 = 0;
      for (int index2 = 0; index2 < numArray.Length; ++index2)
      {
        if (numArray[index2] > numArray[index1])
          index1 = index2;
      }
      if (numArray[index1] == SlotWin)
        query.AddQuery("fights_win", (object) ++p._statistic.fights_win);
      else
        query.AddQuery("fights_lost", (object) ++p._statistic.fights_lost);
      query.AddQuery("fights", (object) ++p._statistic.fights);
      query.AddQuery("totalfights_count", (object) ++p._statistic.totalfights_count);
    }

    public static void UpdateMatchDailyRecordFreeForAll(
      PointBlank.Game.Data.Model.Room Room,
      PointBlank.Game.Data.Model.Account p,
      int SlotWin,
      DBQuery query)
    {
      int[] numArray = new int[16];
      for (int index = 0; index < numArray.Length; ++index)
      {
        PointBlank.Core.Models.Room.Slot slot = Room._slots[index];
        numArray[index] = slot._playerId == 0L ? 0 : slot.allKills;
      }
      int index1 = 0;
      for (int index2 = 0; index2 < numArray.Length; ++index2)
      {
        if (numArray[index2] > numArray[index1])
          index1 = index2;
      }
      if (numArray[index1] == SlotWin)
        query.AddQuery("wins", (object) ++p.Daily.Wins);
      else
        query.AddQuery("loses", (object) ++p.Daily.Loses);
      query.AddQuery("total", (object) ++p.Daily.Total);
    }

    public static void GenerateMissionAwards(PointBlank.Game.Data.Model.Account player, DBQuery query)
    {
      try
      {
        PlayerMissions mission = player._mission;
        int actualMission = mission.actualMission;
        int currentMissionId = mission.getCurrentMissionId();
        int currentCard = mission.getCurrentCard();
        if (currentMissionId <= 0 || mission.selectedCard)
          return;
        int num1 = 0;
        int num2 = 0;
        byte[] currentMissionList = mission.getCurrentMissionList();
        foreach (Card card in MissionCardXml.getCards(currentMissionId, -1))
        {
          if ((int) currentMissionList[card._arrayIdx] >= card._missionLimit)
          {
            ++num2;
            if (card._cardBasicId == currentCard)
              ++num1;
          }
        }
        if (num2 >= 40)
        {
          int blueOrder = player.blue_order;
          int brooch = player.brooch;
          int medal = player.medal;
          int insignia = player.insignia;
          CardAwards award1 = MissionCardXml.getAward(currentMissionId, currentCard);
          if (award1 != null)
          {
            player.brooch += award1._brooch;
            player.medal += award1._medal;
            player.insignia += award1._insignia;
            player._gp += award1._gp;
            player._exp += award1._exp;
          }
          MissionAwards award2 = MissionAwardsXml.getAward(currentMissionId);
          if (award2 != null)
          {
            player.blue_order += award2._blueOrder;
            player._exp += award2._exp;
            player._gp += award2._gp;
          }
          List<ItemsModel> missionAwards = MissionCardXml.getMissionAwards(currentMissionId);
          if (missionAwards.Count > 0)
          {
            foreach (ItemsModel itemsModel in missionAwards)
            {
              if (itemsModel._id != 0)
                player.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, itemsModel));
            }
          }
          player.SendPacket((SendPacket) new PROTOCOL_BASE_QUEST_ACTIVE_IDX_CHANGE_ACK(273U, 4, player));
          if (player.brooch != brooch)
            query.AddQuery("brooch", (object) player.brooch);
          if (player.insignia != insignia)
            query.AddQuery("insignia", (object) player.insignia);
          if (player.medal != medal)
            query.AddQuery("medal", (object) player.medal);
          if (player.blue_order != blueOrder)
            query.AddQuery("blue_order", (object) player.blue_order);
          query.AddQuery("mission_id" + (object) (actualMission + 1), (object) 0);
          ComDiv.updateDB("player_missions", "owner_id", (object) player.player_id, new string[2]
          {
            "card" + (object) (actualMission + 1),
            "mission" + (object) (actualMission + 1)
          }, (object) 0, (object) new byte[0]);
          switch (actualMission)
          {
            case 0:
              mission.mission1 = 0;
              mission.card1 = 0;
              mission.list1 = new byte[40];
              break;
            case 1:
              mission.mission2 = 0;
              mission.card2 = 0;
              mission.list2 = new byte[40];
              break;
            case 2:
              mission.mission3 = 0;
              mission.card3 = 0;
              mission.list3 = new byte[40];
              break;
            case 3:
              mission.mission4 = 0;
              mission.card4 = 0;
              mission.list4 = new byte[40];
              if (player._event == null)
                break;
              player._event.LastQuestFinish = 1;
              ComDiv.updateDB("player_events", "last_quest_finish", (object) 1, "player_id", (object) player.player_id);
              break;
          }
        }
        else
        {
          if (num1 != 4 || mission.selectedCard)
            return;
          CardAwards award = MissionCardXml.getAward(currentMissionId, currentCard);
          if (award != null)
          {
            int brooch = player.brooch;
            int medal = player.medal;
            int insignia = player.insignia;
            player.brooch += award._brooch;
            player.medal += award._medal;
            player.insignia += award._insignia;
            player._gp += award._gp;
            player._exp += award._exp;
            if (player.brooch != brooch)
              query.AddQuery("brooch", (object) player.brooch);
            if (player.insignia != insignia)
              query.AddQuery("insignia", (object) player.insignia);
            if (player.medal != medal)
              query.AddQuery("medal", (object) player.medal);
          }
          mission.selectedCard = true;
          player.SendPacket((SendPacket) new PROTOCOL_BASE_QUEST_ACTIVE_IDX_CHANGE_ACK(1U, 1, player));
        }
      }
      catch
      {
        Logger.error("AllUtils.GenerateMissionAwards");
      }
    }

    public static void ResetSlotInfo(PointBlank.Game.Data.Model.Room room, PointBlank.Core.Models.Room.Slot slot, bool updateInfo)
    {
      if (slot.state < SlotState.LOAD)
        return;
      room.changeSlotState(slot, SlotState.NORMAL, updateInfo);
      slot.ResetSlot();
    }

    public static void votekickResult(PointBlank.Game.Data.Model.Room room)
    {
      PointBlank.Core.Models.Room.VoteKick votekick = room.votekick;
      if (votekick != null)
      {
        int inGamePlayers = votekick.GetInGamePlayers();
        if (votekick.kikar > votekick.deixar && votekick.enemys > 0 && (votekick.allies > 0 && votekick._votes.Count >= inGamePlayers / 2))
        {
          PointBlank.Game.Data.Model.Account playerBySlot = room.getPlayerBySlot(votekick.victimIdx);
          if (playerBySlot != null)
          {
            playerBySlot.SendPacket((SendPacket) new PROTOCOL_BATTLE_NOTIFY_BE_KICKED_BY_KICKVOTE_ACK());
            room.kickedPlayers.Add(playerBySlot.player_id);
            room.RemovePlayer(playerBySlot, true, 2);
          }
        }
        uint erro = 0;
        if (votekick.allies == 0)
          erro = 2147488001U;
        else if (votekick.enemys == 0)
          erro = 2147488002U;
        else if (votekick.deixar < votekick.kikar || votekick._votes.Count < inGamePlayers / 2)
          erro = 2147488000U;
        using (PROTOCOL_BATTLE_NOTIFY_KICKVOTE_RESULT_ACK kickvoteResultAck = new PROTOCOL_BATTLE_NOTIFY_KICKVOTE_RESULT_ACK(erro, votekick))
          room.SendPacketToPlayers((SendPacket) kickvoteResultAck, SlotState.BATTLE, 0);
      }
      room.votekick = (PointBlank.Core.Models.Room.VoteKick) null;
    }

    public static void resetBattleInfo(PointBlank.Game.Data.Model.Room room)
    {
      foreach (PointBlank.Core.Models.Room.Slot slot in room._slots)
      {
        if (slot._playerId > 0L && slot.state >= SlotState.LOAD)
        {
          slot.state = SlotState.NORMAL;
          slot.ResetSlot();
        }
      }
      room.blockedClan = false;
      room.rounds = 1;
      room.spawnsCount = 0;
      room._redKills = 0;
      room._redAssists = 0;
      room._redDeaths = 0;
      room._blueKills = 0;
      room._blueAssists = 0;
      room._blueDeaths = 0;
      room.red_dino = 0;
      room.blue_dino = 0;
      room.red_rounds = 0;
      room.blue_rounds = 0;
      room.BattleStart = new DateTime();
      room._timeRoom = 0U;
      room.Bar1 = 0;
      room.Bar2 = 0;
      room.swapRound = false;
      room.IngameAiLevel = (byte) 0;
      room._state = RoomState.Ready;
      room.updateRoomInfo();
      room.votekick = (PointBlank.Core.Models.Room.VoteKick) null;
      room.UdpServer = (BattleServer) null;
      if (room.round.Timer != null)
        room.round.Timer = (Timer) null;
      if (room.vote.Timer != null)
        room.vote.Timer = (Timer) null;
      if (room.bomb.Timer != null)
        room.bomb.Timer = (Timer) null;
      room.updateSlotsInfo();
    }

    public static List<int> getDinossaurs(PointBlank.Game.Data.Model.Room room, bool forceNewTRex, int forceRexIdx)
    {
      List<int> intList = new List<int>();
      if (room.room_type == RoomType.Boss || room.room_type == RoomType.CrossCounter)
      {
        int index = room.rounds == 1 ? 0 : 1;
        foreach (int team in room.GetTeamArray(index))
        {
          PointBlank.Core.Models.Room.Slot slot = room._slots[team];
          if (slot.state == SlotState.BATTLE && !slot.specGM)
            intList.Add(team);
        }
        if (((room.TRex == -1 ? 1 : (room._slots[room.TRex].state <= SlotState.BATTLE_READY ? 1 : 0)) | (forceNewTRex ? 1 : 0)) != 0 && intList.Count > 1 && room.room_type == RoomType.Boss)
        {
          if (forceRexIdx >= 0 && intList.Contains(forceRexIdx))
            room.TRex = forceRexIdx;
          else if (forceRexIdx == -2)
            room.TRex = intList[new Random().Next(0, intList.Count)];
        }
      }
      return intList;
    }

    public static void BattleEndPlayersCount(PointBlank.Game.Data.Model.Room room, bool isBotMode)
    {
      if (room == null | isBotMode || !room.isPreparing())
        return;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      foreach (PointBlank.Core.Models.Room.Slot slot in room._slots)
      {
        if (slot.state == SlotState.BATTLE)
        {
          if (slot._team == 0)
            ++num2;
          else
            ++num1;
        }
        else if (slot.state >= SlotState.LOAD)
        {
          if (slot._team == 0)
            ++num4;
          else
            ++num3;
        }
      }
      if ((num2 != 0 && num1 != 0 || room._state != RoomState.Battle) && (num4 != 0 && num3 != 0 || room._state > RoomState.PreBattle))
        return;
      AllUtils.EndBattle(room, isBotMode);
    }

    public static void EndBattle(PointBlank.Game.Data.Model.Room room)
    {
      AllUtils.EndBattle(room, room.isBotMode());
    }

    public static void EndBattle(PointBlank.Game.Data.Model.Room room, bool isBotMode)
    {
      TeamResultType winnerTeam = AllUtils.GetWinnerTeam(room);
      AllUtils.EndBattle(room, isBotMode, winnerTeam);
    }

    public static void EndBattleNoPoints(PointBlank.Game.Data.Model.Room room)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
      if (allPlayers.Count != 0)
      {
        ushort MissionFlag;
        ushort SlotFlag;
        AllUtils.getBattleResult(room, out MissionFlag, out SlotFlag);
        bool BotMode = room.isBotMode();
        foreach (PointBlank.Game.Data.Model.Account p in allPlayers)
          p.SendPacket((SendPacket) new PROTOCOL_BATTLE_ENDBATTLE_ACK(p, TeamResultType.TeamDraw, SlotFlag, MissionFlag, BotMode));
      }
      AllUtils.resetBattleInfo(room);
    }

    public static void EndBattle(PointBlank.Game.Data.Model.Room room, bool isBotMode, TeamResultType winnerTeam)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
      if (allPlayers.Count != 0)
      {
        room.CalculateResult(winnerTeam, isBotMode);
        ushort MissionFlag;
        ushort SlotFlag;
        AllUtils.getBattleResult(room, out MissionFlag, out SlotFlag);
        foreach (PointBlank.Game.Data.Model.Account p in allPlayers)
          p.SendPacket((SendPacket) new PROTOCOL_BATTLE_ENDBATTLE_ACK(p, winnerTeam, SlotFlag, MissionFlag, isBotMode));
      }
      AllUtils.resetBattleInfo(room);
    }

    public static int percentage(int total, int percent)
    {
      return total * percent / 100;
    }

    public static void BattleEndRound(PointBlank.Game.Data.Model.Room room, int winner, bool forceRestart)
    {
      int roundsByMask = room.getRoundsByMask();
      if (room.red_rounds >= roundsByMask || room.blue_rounds >= roundsByMask)
      {
        room.StopBomb();
        using (PROTOCOL_BATTLE_MISSION_ROUND_END_ACK missionRoundEndAck = new PROTOCOL_BATTLE_MISSION_ROUND_END_ACK(room, winner, RoundEndType.AllDeath))
          room.SendPacketToPlayers((SendPacket) missionRoundEndAck, SlotState.BATTLE, 0);
        AllUtils.EndBattle(room, room.isBotMode(), (TeamResultType) winner);
      }
      else
      {
        if (!(!room.C4_actived | forceRestart))
          return;
        room.StopBomb();
        ++room.rounds;
        GameSync.SendUDPRoundSync(room);
        using (PROTOCOL_BATTLE_MISSION_ROUND_END_ACK missionRoundEndAck = new PROTOCOL_BATTLE_MISSION_ROUND_END_ACK(room, winner, RoundEndType.AllDeath))
          room.SendPacketToPlayers((SendPacket) missionRoundEndAck, SlotState.BATTLE, 0);
        room.RoundRestart();
      }
    }

    public static void BattleEndRound(PointBlank.Game.Data.Model.Room room, int winner, RoundEndType motive)
    {
      using (PROTOCOL_BATTLE_MISSION_ROUND_END_ACK missionRoundEndAck = new PROTOCOL_BATTLE_MISSION_ROUND_END_ACK(room, winner, motive))
        room.SendPacketToPlayers((SendPacket) missionRoundEndAck, SlotState.BATTLE, 0);
      room.StopBomb();
      int roundsByMask = room.getRoundsByMask();
      if (room.red_rounds >= roundsByMask || room.blue_rounds >= roundsByMask)
      {
        AllUtils.EndBattle(room, room.isBotMode(), (TeamResultType) winner);
      }
      else
      {
        ++room.rounds;
        GameSync.SendUDPRoundSync(room);
        room.RoundRestart();
      }
    }

    public static int AddFriend(PointBlank.Game.Data.Model.Account owner, PointBlank.Game.Data.Model.Account friend, int state)
    {
      if (owner != null)
      {
        if (friend != null)
        {
          try
          {
            Friend friend1 = owner.FriendSystem.GetFriend(friend.player_id);
            if (friend1 == null)
            {
              using (NpgsqlConnection npgsqlConnection = SqlConnection.getInstance().conn())
              {
                NpgsqlCommand command = npgsqlConnection.CreateCommand();
                npgsqlConnection.Open();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@friend", (object) friend.player_id);
                command.Parameters.AddWithValue("@owner", (object) owner.player_id);
                command.Parameters.AddWithValue("@state", (object) state);
                command.CommandText = "INSERT INTO friends(friend_id,owner_id,state)VALUES(@friend,@owner,@state)";
                command.ExecuteNonQuery();
                command.Dispose();
                npgsqlConnection.Dispose();
                npgsqlConnection.Close();
              }
              lock (owner.FriendSystem._friends)
              {
                Friend friend2 = new Friend(friend.player_id, friend._rank, friend.name_color, friend.player_name, friend._isOnline, friend._status)
                {
                  state = state,
                  removed = false
                };
                owner.FriendSystem._friends.Add(friend2);
                SendFriendInfo.Load(owner, friend2, 0);
              }
              return 0;
            }
            if (friend1.removed)
            {
              friend1.removed = false;
              PlayerManager.UpdateFriendBlock(owner.player_id, friend1);
              SendFriendInfo.Load(owner, friend1, 1);
            }
            return 1;
          }
          catch (Exception ex)
          {
            Logger.info("[AllUtils.AddFriend] " + ex.ToString());
            return -1;
          }
        }
      }
      return -1;
    }

    public static void syncPlayerToFriends(PointBlank.Game.Data.Model.Account p, bool all)
    {
      if (p == null || p.FriendSystem._friends.Count == 0)
        return;
      PlayerInfo playerInfo = new PlayerInfo(p.player_id, p._rank, p.name_color, p.player_name, p._isOnline, p._status);
      for (int index1 = 0; index1 < p.FriendSystem._friends.Count; ++index1)
      {
        Friend friend1 = p.FriendSystem._friends[index1];
        if (all || friend1.state == 0 && !friend1.removed)
        {
          PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(friend1.player_id, 32);
          if (account != null)
          {
            int index2 = -1;
            Friend friend2 = account.FriendSystem.GetFriend(p.player_id, out index2);
            if (friend2 != null)
            {
              friend2.player = playerInfo;
              account.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(FriendChangeState.Update, friend2, index2), false);
            }
          }
        }
      }
    }

    public static void syncPlayerToClanMembers(PointBlank.Game.Data.Model.Account player)
    {
      if (player == null || player.clanId <= 0)
        return;
      using (PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK memberInfoChangeAck = new PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK(player))
        ClanManager.SendPacket((SendPacket) memberInfoChangeAck, player.clanId, player.player_id, true, true);
    }

    public static void updateSlotEquips(PointBlank.Game.Data.Model.Account p)
    {
      PointBlank.Game.Data.Model.Room room = p._room;
      if (room == null)
        return;
      AllUtils.updateSlotEquips(p, room);
    }

    public static void updateSlotEquips(PointBlank.Game.Data.Model.Account p, PointBlank.Game.Data.Model.Room room)
    {
      PointBlank.Core.Models.Room.Slot slot;
      if (!room.getSlot(p._slotId, out slot))
        return;
      slot._equip = p._equip;
    }

    public static ushort getSlotsFlag(PointBlank.Game.Data.Model.Room Room, bool OnlyNoSpectators, bool MissionSuccess)
    {
      if (Room == null)
        return 0;
      int num = 0;
      foreach (PointBlank.Core.Models.Room.Slot slot in Room._slots)
      {
        if (slot.state >= SlotState.LOAD && (MissionSuccess && slot.MissionsCompleted || !MissionSuccess && (!OnlyNoSpectators || !slot.espectador)))
          num += slot.Flag;
      }
      return (ushort) num;
    }

    public static void getBattleResult(PointBlank.Game.Data.Model.Room Room, out ushort MissionFlag, out ushort SlotFlag)
    {
      MissionFlag = (ushort) 0;
      SlotFlag = (ushort) 0;
      if (Room == null)
        return;
      foreach (PointBlank.Core.Models.Room.Slot slot in Room._slots)
      {
        if (slot.state >= SlotState.LOAD)
        {
          ushort flag = (ushort) slot.Flag;
          if (slot.MissionsCompleted)
            MissionFlag += flag;
          SlotFlag += flag;
        }
      }
    }

    public static void DiscountPlayerItems(PointBlank.Core.Models.Room.Slot slot, PointBlank.Game.Data.Model.Account p)
    {
      long int64 = Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmm"));
      bool flag1 = false;
      bool flag2 = false;
      List<ItemsModel> itemsModelList = new List<ItemsModel>();
      List<object> objectList1 = new List<object>();
      List<object> objectList2 = new List<object>();
      PlayerBonus bonus = p._bonus;
      int num1 = bonus != null ? bonus.bonuses : 0;
      int num2 = bonus != null ? bonus.freepass : 0;
      lock (p._inventory._items)
      {
        for (int index1 = 0; index1 < p._inventory._items.Count; ++index1)
        {
          ItemsModel itemsModel = p._inventory._items[index1];
          if (itemsModel._equip == 1 && slot.armas_usadas.Contains(itemsModel._id) && !slot.specGM)
          {
            if (--itemsModel._count < 1L)
            {
              objectList1.Add((object) itemsModel._objId);
              p._inventory._items.RemoveAt(index1--);
            }
            else
            {
              itemsModelList.Add(itemsModel);
              ComDiv.updateDB("player_items", "count", (object) itemsModel._count, "object_id", (object) itemsModel._objId, "owner_id", (object) p.player_id);
            }
          }
          else if (itemsModel._count <= int64 & itemsModel._equip == 2)
          {
            if (itemsModel._category == 3 && ComDiv.getIdStatics(itemsModel._id, 1) == 16)
            {
              if (bonus != null)
              {
                if (!bonus.RemoveBonuses(itemsModel._id))
                {
                  if (itemsModel._id == 1600014)
                  {
                    ComDiv.updateDB("player_bonus", "sightcolor", (object) 4, "player_id", (object) p.player_id);
                    bonus.sightColor = 4;
                    flag1 = true;
                  }
                  else if (itemsModel._id == 1600009)
                  {
                    ComDiv.updateDB("player_bonus", "fakerank", (object) 55, "player_id", (object) p.player_id);
                    bonus.fakeRank = 55;
                    flag1 = true;
                  }
                }
                else if (itemsModel._id == 1600006)
                {
                  ComDiv.updateDB("accounts", "name_color", (object) 0, "player_id", (object) p.player_id);
                  p.name_color = 0;
                  if (p._room != null)
                  {
                    using (PROTOCOL_ROOM_GET_NICKNAME_ACK roomGetNicknameAck = new PROTOCOL_ROOM_GET_NICKNAME_ACK(slot._id, p.player_name, p.name_color))
                      p._room.SendPacketToPlayers((SendPacket) roomGetNicknameAck);
                  }
                }
                else if (itemsModel._id == 1600187)
                {
                  ComDiv.updateDB("player_bonus", "muzzle", (object) 0, "player_id", (object) p.player_id);
                  bonus.muzzle = 0;
                  flag1 = true;
                }
                CouponFlag couponEffect = CouponEffectManager.getCouponEffect(itemsModel._id);
                if (couponEffect != null && couponEffect.EffectFlag > (CouponEffects) 0 && p.effects.HasFlag((Enum) couponEffect.EffectFlag))
                {
                  p.effects -= couponEffect.EffectFlag;
                  flag2 = true;
                }
              }
              else
                continue;
            }
            else if (itemsModel._category == 2 && ComDiv.getIdStatics(itemsModel._id, 1) == 6)
            {
              Character character1 = p.getCharacter(itemsModel._id);
              if (character1 != null)
              {
                int Slot = 0;
                for (int index2 = 0; index2 < p.Characters.Count; ++index2)
                {
                  Character character2 = p.Characters[index2];
                  if (character2.Slot != character1.Slot)
                  {
                    character2.Slot = Slot;
                    CharacterManager.Update(Slot, character2.ObjId);
                    ++Slot;
                  }
                }
                p.SendPacket((SendPacket) new PROTOCOL_CHAR_DELETE_CHARA_ACK(0U, character1.Slot, p, itemsModel));
                if (CharacterManager.Delete(character1.ObjId, p.player_id))
                  p.Characters.Remove(character1);
              }
            }
            if (ComDiv.getIdStatics(itemsModel._id, 1) == 6)
              objectList2.Add((object) itemsModel._objId);
            else
              objectList1.Add((object) itemsModel._objId);
            p._inventory._items.RemoveAt(index1--);
          }
        }
      }
      if (bonus != null && (bonus.bonuses != num1 || bonus.freepass != num2))
        PlayerManager.updatePlayerBonus(p.player_id, bonus.bonuses, bonus.freepass);
      if (p.effects < (CouponEffects) 0)
        p.effects = (CouponEffects) 0;
      if (flag2)
        PlayerManager.updateCupomEffects(p.player_id, p.effects);
      if (itemsModelList.Count > 0)
      {
        for (int index = 0; index < itemsModelList.Count; ++index)
        {
          ItemsModel itemsModel = itemsModelList[index];
          if (itemsModel._id != 0)
            p.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(1, p, itemsModel));
        }
      }
      for (int index = 0; index < objectList1.Count; ++index)
      {
        long objId = (long) objectList1[index];
        p.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_DELETE_ITEM_ACK(1U, objId));
      }
      ComDiv.deleteDB("player_items", "object_id", objectList1.ToArray(), "owner_id", (object) p.player_id);
      ComDiv.deleteDB("player_items", "object_id", objectList2.ToArray(), "owner_id", (object) p.player_id);
      if (flag1)
        p.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(0, p));
      if (p._equip == null)
        Logger.warning("This is Null.");
      if (PlayerManager.CheckEquipedItems(p._equip, p._inventory._items, false) <= 0)
        return;
      p.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_CHANGE_INVENTORY_ACK(p));
      slot._equip = p._equip;
    }

    public static void TryBalancePlayer(PointBlank.Game.Data.Model.Room room, PointBlank.Game.Data.Model.Account player, bool inBattle, ref PointBlank.Core.Models.Room.Slot mySlot)
    {
      PointBlank.Core.Models.Room.Slot slot1 = room.getSlot(player._slotId);
      if (slot1 == null)
        return;
      int team = slot1._team;
      int balanceTeamIdx = AllUtils.getBalanceTeamIdx(room, inBattle, team);
      if (team == balanceTeamIdx || balanceTeamIdx == -1)
        return;
      PointBlank.Core.Models.Room.Slot slot2 = (PointBlank.Core.Models.Room.Slot) null;
      foreach (int index in team == 1 ? room.RED_TEAM : room.BLUE_TEAM)
      {
        PointBlank.Core.Models.Room.Slot slot3 = room._slots[index];
        if (slot3.state != SlotState.CLOSE && slot3._playerId == 0L)
        {
          slot2 = slot3;
          break;
        }
      }
      if (slot2 == null)
        return;
      List<SlotChange> slots = new List<SlotChange>();
      lock (room._slots)
        room.SwitchSlots(slots, slot2._id, slot1._id, false);
      if (slots.Count <= 0)
        return;
      player._slotId = slot1._id;
      mySlot = slot1;
      using (PROTOCOL_ROOM_TEAM_BALANCE_ACK roomTeamBalanceAck = new PROTOCOL_ROOM_TEAM_BALANCE_ACK(slots, room._leader, 1))
        room.SendPacketToPlayers((SendPacket) roomTeamBalanceAck);
      room.updateSlotsInfo();
    }

    public static int getBalanceTeamIdx(PointBlank.Game.Data.Model.Room room, bool inBattle, int PlayerTeamIdx)
    {
      int num1 = !inBattle || PlayerTeamIdx != 0 ? 0 : 1;
      int num2 = !inBattle || PlayerTeamIdx != 1 ? 0 : 1;
      foreach (PointBlank.Core.Models.Room.Slot slot in room._slots)
      {
        if (slot.state == SlotState.NORMAL && !inBattle || slot.state >= SlotState.LOAD & inBattle)
        {
          if (slot._team == 0)
            ++num1;
          else
            ++num2;
        }
      }
      if (num1 + 1 < num2)
        return 0;
      return num2 + 1 >= num1 + 1 ? -1 : 1;
    }

    public static int getNewSlotId(int slotIdx)
    {
      if (slotIdx % 2 != 0)
        return slotIdx - 1;
      return slotIdx + 1;
    }

    public static void GetXmasReward(PointBlank.Game.Data.Model.Account p)
    {
      EventXmasModel runningEvent = EventXmasSyncer.getRunningEvent();
      if (runningEvent == null)
        return;
      PlayerEvent playerEvent = p._event;
      uint num = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
      if (playerEvent == null || playerEvent.LastXmasRewardDate > runningEvent.startDate && playerEvent.LastXmasRewardDate <= runningEvent.endDate || !ComDiv.updateDB("player_events", "last_xmas_reward_date", (object) (long) num, "player_id", (object) p.player_id))
        return;
      playerEvent.LastXmasRewardDate = num;
      p.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, p, new ItemsModel(702001024, 1, "Xmas Event", 1, 100L, 0L)));
    }

    public static void BattleEndRoundPlayersCount(PointBlank.Game.Data.Model.Room room)
    {
      if (room.round.Timer != null || room.room_type != RoomType.Bomb && room.room_type != RoomType.Annihilation && room.room_type != RoomType.Convoy)
        return;
      int RedPlayers;
      int BluePlayers;
      int RedDeaths;
      int BlueDeaths;
      room.getPlayingPlayers(true, out RedPlayers, out BluePlayers, out RedDeaths, out BlueDeaths);
      if (RedDeaths == RedPlayers)
      {
        if (!room.C4_actived)
          ++room.blue_rounds;
        AllUtils.BattleEndRound(room, 1, false);
      }
      else
      {
        if (BlueDeaths != BluePlayers)
          return;
        ++room.red_rounds;
        AllUtils.BattleEndRound(room, 0, true);
      }
    }

    public static void BattleEndKills(PointBlank.Game.Data.Model.Room room)
    {
      AllUtils.BaseEndByKills(room, room.isBotMode());
    }

    public static void BattleEndKills(PointBlank.Game.Data.Model.Room room, bool isBotMode)
    {
      AllUtils.BaseEndByKills(room, isBotMode);
    }

    private static void BaseEndByKills(PointBlank.Game.Data.Model.Room room, bool isBotMode)
    {
      int killsByMask = room.getKillsByMask();
      if (room._redKills < killsByMask && room._blueKills < killsByMask)
        return;
      List<PointBlank.Game.Data.Model.Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
      if (allPlayers.Count != 0)
      {
        TeamResultType winnerTeam = AllUtils.GetWinnerTeam(room);
        room.CalculateResult(winnerTeam, isBotMode);
        ushort MissionFlag;
        ushort SlotFlag;
        AllUtils.getBattleResult(room, out MissionFlag, out SlotFlag);
        using (PROTOCOL_BATTLE_MISSION_ROUND_END_ACK missionRoundEndAck = new PROTOCOL_BATTLE_MISSION_ROUND_END_ACK(room, winnerTeam, RoundEndType.TimeOut))
        {
          byte[] completeBytes = missionRoundEndAck.GetCompleteBytes("AllUtils.BaseEndByKills");
          foreach (PointBlank.Game.Data.Model.Account p in allPlayers)
          {
            PointBlank.Core.Models.Room.Slot slot = room.getSlot(p._slotId);
            if (slot != null)
            {
              if (slot.state == SlotState.BATTLE)
                p.SendCompletePacket(completeBytes);
              p.SendPacket((SendPacket) new PROTOCOL_BATTLE_ENDBATTLE_ACK(p, winnerTeam, SlotFlag, MissionFlag, isBotMode));
            }
          }
        }
      }
      AllUtils.resetBattleInfo(room);
    }

    public static void BattleEndKillsFreeForAll(PointBlank.Game.Data.Model.Room room)
    {
      AllUtils.BaseEndByKillsFreeForAll(room);
    }

    private static void BaseEndByKillsFreeForAll(PointBlank.Game.Data.Model.Room room)
    {
      int killsByMask = room.getKillsByMask();
      int[] numArray = new int[16];
      for (int index = 0; index < numArray.Length; ++index)
      {
        PointBlank.Core.Models.Room.Slot slot = room._slots[index];
        numArray[index] = slot._playerId == 0L ? 0 : slot.allKills;
      }
      int index1 = 0;
      for (int index2 = 0; index2 < numArray.Length; ++index2)
      {
        if (numArray[index2] > numArray[index1])
          index1 = index2;
      }
      int num = index1;
      if (numArray[index1] < killsByMask)
        return;
      List<PointBlank.Game.Data.Model.Account> allPlayers = room.getAllPlayers(SlotState.READY, 1);
      if (allPlayers.Count != 0)
      {
        room.CalculateResultFreeForAll(num);
        ushort MissionFlag;
        ushort SlotFlag;
        AllUtils.getBattleResult(room, out MissionFlag, out SlotFlag);
        using (PROTOCOL_BATTLE_MISSION_ROUND_END_ACK missionRoundEndAck = new PROTOCOL_BATTLE_MISSION_ROUND_END_ACK(room, num, RoundEndType.FreeForAll))
        {
          byte[] completeBytes = missionRoundEndAck.GetCompleteBytes("AllUtils.BaseEndByKills");
          foreach (PointBlank.Game.Data.Model.Account p in allPlayers)
          {
            PointBlank.Core.Models.Room.Slot slot = room.getSlot(p._slotId);
            if (slot != null)
            {
              if (slot.state == SlotState.BATTLE)
                p.SendCompletePacket(completeBytes);
              p.SendPacket((SendPacket) new PROTOCOL_BATTLE_ENDBATTLE_ACK(p, num, SlotFlag, MissionFlag, false));
            }
          }
        }
      }
      AllUtils.resetBattleInfo(room);
    }

    public static bool CheckClanMatchRestrict(PointBlank.Game.Data.Model.Room room)
    {
      if (room._channelType == 4)
      {
        foreach (ClanModel clanModel in (IEnumerable<ClanModel>) AllUtils.GetClanListMatchPlayers(room).Values)
        {
          if (clanModel.RedPlayers >= 1 && clanModel.BluePlayers >= 1)
          {
            room.blockedClan = true;
            return true;
          }
        }
      }
      return false;
    }

    public static bool Have2ClansToClanMatch(PointBlank.Game.Data.Model.Room room)
    {
      return AllUtils.GetClanListMatchPlayers(room).Count == 2;
    }

    public static bool HavePlayersToClanMatch(PointBlank.Game.Data.Model.Room room)
    {
      SortedList<int, ClanModel> listMatchPlayers = AllUtils.GetClanListMatchPlayers(room);
      bool flag1 = false;
      bool flag2 = false;
      foreach (ClanModel clanModel in (IEnumerable<ClanModel>) listMatchPlayers.Values)
      {
        if (clanModel.RedPlayers >= 4)
          flag1 = true;
        else if (clanModel.BluePlayers >= 4)
          flag2 = true;
      }
      return flag1 & flag2;
    }

    private static SortedList<int, ClanModel> GetClanListMatchPlayers(PointBlank.Game.Data.Model.Room room)
    {
      SortedList<int, ClanModel> sortedList = new SortedList<int, ClanModel>();
      for (int index = 0; index < room.getAllPlayers().Count; ++index)
      {
        PointBlank.Game.Data.Model.Account allPlayer = room.getAllPlayers()[index];
        if (allPlayer.clanId != 0)
        {
          ClanModel clanModel;
          if (sortedList.TryGetValue(allPlayer.clanId, out clanModel) && clanModel != null)
          {
            if (allPlayer._slotId % 2 == 0)
              ++clanModel.RedPlayers;
            else
              ++clanModel.BluePlayers;
          }
          else
          {
            clanModel = new ClanModel();
            clanModel.clanId = allPlayer.clanId;
            if (allPlayer._slotId % 2 == 0)
              ++clanModel.RedPlayers;
            else
              ++clanModel.BluePlayers;
            sortedList.Add(allPlayer.clanId, clanModel);
          }
        }
      }
      return sortedList;
    }

    public static void PlayTimeEvent(
      long playedTime,
      PointBlank.Game.Data.Model.Account p,
      PlayTimeModel ptEvent,
      bool isBotMode)
    {
      PointBlank.Game.Data.Model.Room room = p._room;
      PlayerEvent pE = p._event;
      if (room == null | isBotMode || pE == null)
        return;
      long lastPlaytimeValue = pE.LastPlaytimeValue;
      long lastPlaytimeFinish = (long) pE.LastPlaytimeFinish;
      long lastPlaytimeDate = (long) pE.LastPlaytimeDate;
      if (pE.LastPlaytimeDate < ptEvent._startDate)
      {
        pE.LastPlaytimeFinish = 0;
        pE.LastPlaytimeValue = 0L;
      }
      if (pE.LastPlaytimeFinish == 0)
      {
        pE.LastPlaytimeValue += playedTime;
        if (pE.LastPlaytimeValue >= ptEvent._time)
          pE.LastPlaytimeFinish = 1;
        pE.LastPlaytimeDate = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
        if (pE.LastPlaytimeValue >= ptEvent._time)
          p.SendPacket((SendPacket) new PROTOCOL_BATTLE_PLAYER_TIME_ACK(0, ptEvent));
        else
          p.SendPacket((SendPacket) new PROTOCOL_BATTLE_PLAYER_TIME_ACK(1, new PlayTimeModel()
          {
            _time = ptEvent._time - pE.LastPlaytimeValue
          }));
      }
      else if (pE.LastPlaytimeFinish == 1)
        p.SendPacket((SendPacket) new PROTOCOL_BATTLE_PLAYER_TIME_ACK(0, ptEvent));
      if (pE.LastPlaytimeValue == lastPlaytimeValue && (long) pE.LastPlaytimeFinish == lastPlaytimeFinish && (long) pE.LastPlaytimeDate == lastPlaytimeDate)
        return;
      EventPlayTimeSyncer.ResetPlayerEvent(p.player_id, pE);
    }
  }
}
