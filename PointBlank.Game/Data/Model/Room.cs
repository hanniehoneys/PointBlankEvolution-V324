using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Account.Rank;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Map;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Sync;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PointBlank.Game.Data.Model
{
  public class Room
  {
    public PointBlank.Core.Models.Room.Slot[] _slots = new PointBlank.Core.Models.Room.Slot[16];
    public int rounds = 1;
    public int TRex = -1;
    public int _ping = 5;
    public byte aiCount = 1;
    public readonly int[] TIMES = new int[11]
    {
      3,
      3,
      3,
      5,
      7,
      5,
      10,
      15,
      20,
      25,
      30
    };
    public readonly int[] KILLS = new int[9]
    {
      15,
      30,
      50,
      60,
      80,
      100,
      120,
      140,
      160
    };
    public readonly int[] ROUNDS = new int[6]
    {
      1,
      2,
      3,
      5,
      7,
      9
    };
    public readonly int[] RED_TEAM = new int[8]
    {
      0,
      2,
      4,
      6,
      8,
      10,
      12,
      14
    };
    public readonly int[] BLUE_TEAM = new int[8]
    {
      1,
      3,
      5,
      7,
      9,
      11,
      13,
      15
    };
    public byte[] HitParts = new byte[35];
    public byte[] DefaultParts = new byte[35]
    {
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 4,
      (byte) 5,
      (byte) 6,
      (byte) 7,
      (byte) 8,
      (byte) 9,
      (byte) 10,
      (byte) 11,
      (byte) 12,
      (byte) 13,
      (byte) 14,
      (byte) 15,
      (byte) 16,
      (byte) 17,
      (byte) 18,
      (byte) 19,
      (byte) 20,
      (byte) 21,
      (byte) 22,
      (byte) 23,
      (byte) 24,
      (byte) 25,
      (byte) 26,
      (byte) 27,
      (byte) 28,
      (byte) 29,
      (byte) 30,
      (byte) 31,
      (byte) 32,
      (byte) 33,
      (byte) 34
    };
    public DateTime LastPingSync = DateTime.Now;
    public TimerState bomb = new TimerState();
    public TimerState countdown = new TimerState();
    public TimerState round = new TimerState();
    public TimerState vote = new TimerState();
    public SafeList<long> kickedPlayers = new SafeList<long>();
    public SafeList<long> requestHost = new SafeList<long>();
    public List<GameRule> GameRules = new List<GameRule>();
    public int _channelType;
    public int blue_rounds;
    public int blue_dino;
    public int red_rounds;
    public int red_dino;
    public int Bar1;
    public int Bar2;
    public int _redKills;
    public int _redDeaths;
    public int _redAssists;
    public int _blueKills;
    public int _blueDeaths;
    public int _blueAssists;
    public int spawnsCount;
    public int rule;
    public int killtime;
    public int _roomId;
    public int _channelId;
    public int _leader;
    public byte Limit;
    public byte WatchRuleFlag;
    public byte IngameAiLevel;
    public byte aiLevel;
    public byte aiType;
    public byte stage;
    public short BalanceType;
    public uint _timeRoom;
    public uint StartDate;
    public uint UniqueRoomId;
    public uint Seed;
    public long StartTick;
    public string name;
    public string password;
    public string _mapName;
    public PointBlank.Core.Models.Room.VoteKick votekick;
    public MapIdEnum mapId;
    public RoomType room_type;
    public RoomState _state;
    public GameRuleFlag RuleFlag;
    public RoomStageFlag Flag;
    public RoomWeaponsFlag weaponsFlag;
    public bool C4_actived;
    public bool swapRound;
    public bool changingSlots;
    public bool blockedClan;
    public bool ShotgunMode;
    public bool SniperMode;
    public BattleServer UdpServer;
    public DateTime BattleStart;
    public bool GameRuleActive;
    public bool ShotgunActive;
    public bool BarrettActive;
    public bool MaskActive;

    public Room(int roomId, Channel ch)
    {
      this._roomId = roomId;
      for (int slotIdx = 0; slotIdx < this._slots.Length; ++slotIdx)
        this._slots[slotIdx] = new PointBlank.Core.Models.Room.Slot(slotIdx);
      this._channelId = ch._id;
      this._channelType = ch._type;
      this.SetUniqueId();
      this.GameRules = GameRuleManager.getGameRules(GameConfig.ruleId);
    }

    public bool thisModeHaveCD()
    {
      RoomType roomType = this.room_type;
      switch (roomType)
      {
        case RoomType.Bomb:
        case RoomType.Annihilation:
        case RoomType.Boss:
        case RoomType.CrossCounter:
          return true;
        default:
          return roomType == RoomType.Convoy;
      }
    }

    public bool thisModeHaveRounds()
    {
      RoomType roomType = this.room_type;
      switch (roomType)
      {
        case RoomType.Bomb:
        case RoomType.Destroy:
        case RoomType.Annihilation:
        case RoomType.Defense:
          return true;
        default:
          return roomType == RoomType.Convoy;
      }
    }

    public int getFlag()
    {
      int num = 0;
      if (this.Flag.HasFlag((Enum) RoomStageFlag.RANDOM_MAP))
        num += 2;
      if (this.Flag.HasFlag((Enum) RoomStageFlag.PASSWORD) || this.password.Length > 0)
        num += 4;
      if (this.BalanceType == (short) 1)
        num += 32;
      if (this.Limit > (byte) 0 && this._state > RoomState.Ready)
        num += 128;
      this.Flag = (RoomStageFlag) num;
      return num;
    }

    public void LoadHitParts()
    {
      int next = new Random().Next(34);
      byte[] array = ((IEnumerable<byte>) this.DefaultParts).OrderBy<byte, bool>((Func<byte, bool>) (x => (int) x <= next)).ToArray<byte>();
      Logger.warning("Idx: " + (object) next + "/ Hits: " + BitConverter.ToString(array));
      this.HitParts = array;
      byte[] numArray = new byte[35];
      for (int index = 0; index < 35; ++index)
      {
        byte hitPart = this.HitParts[index];
        numArray[(index + 8) % 35] = hitPart;
      }
      Logger.warning("Array: " + BitConverter.ToString(numArray));
    }

    private void SetUniqueId()
    {
      this.UniqueRoomId = (uint) ((GameConfig.serverId & (int) byte.MaxValue) << 20 | (this._channelId & (int) byte.MaxValue) << 12 | this._roomId & 4095);
    }

    public void SetSeed()
    {
      this.Seed = (uint) ((RoomType) ((int) (this.mapId & (MapIdEnum) 255) << 20 | (this.rule & (int) byte.MaxValue) << 12) | this.room_type & (RoomType) 4095);
    }

    public void SetBotLevel()
    {
      if (!this.isBotMode())
        return;
      this.IngameAiLevel = this.aiLevel;
      for (int index = 0; index < 16; ++index)
        this._slots[index].aiLevel = (int) this.IngameAiLevel;
    }

    public bool isBotMode()
    {
      if (this.stage != (byte) 2 && this.stage != (byte) 4)
        return this.stage == (byte) 6;
      return true;
    }

    private void SetSpecialStage()
    {
      if (this.room_type == RoomType.Defense)
      {
        if (this.mapId != MapIdEnum.BlackPanther)
          return;
        this.Bar1 = 6000;
        this.Bar2 = 9000;
      }
      else
      {
        if (this.room_type != RoomType.Destroy)
          return;
        if (this.mapId == MapIdEnum.Hospital)
        {
          this.Bar1 = 12000;
          this.Bar2 = 12000;
        }
        else
        {
          if (this.mapId != MapIdEnum.BreakDown)
            return;
          this.Bar1 = 6000;
          this.Bar2 = 6000;
        }
      }
    }

    public int getInBattleTime()
    {
      int num = 0;
      if (this.BattleStart != new DateTime() && (this._state == RoomState.Battle || this._state == RoomState.PreBattle))
      {
        num = (int) (DateTime.Now - this.BattleStart).TotalSeconds;
        if (num < 0)
          num = 0;
      }
      return num;
    }

    public int getInBattleTimeLeft()
    {
      return this.getTimeByMask() * 60 - this.getInBattleTime();
    }

    public Channel getChannel()
    {
      return ChannelsXml.getChannel(this._channelId);
    }

    public bool getChannel(out Channel ch)
    {
      ch = ChannelsXml.getChannel(this._channelId);
      return ch != null;
    }

    public bool getSlot(int slotIdx, out PointBlank.Core.Models.Room.Slot slot)
    {
      slot = (PointBlank.Core.Models.Room.Slot) null;
      lock (this._slots)
      {
        if (slotIdx >= 0 && slotIdx <= 15)
          slot = this._slots[slotIdx];
        return slot != null;
      }
    }

    public PointBlank.Core.Models.Room.Slot getSlot(int slotIdx)
    {
      lock (this._slots)
      {
        if (slotIdx >= 0 && slotIdx <= 15)
          return this._slots[slotIdx];
        return (PointBlank.Core.Models.Room.Slot) null;
      }
    }

    public void StartCounter(int type, PointBlank.Game.Data.Model.Account player, PointBlank.Core.Models.Room.Slot slot)
    {
      EventErrorEnum error = EventErrorEnum.SUCCESS;
      int period;
      switch (type)
      {
        case 0:
          error = EventErrorEnum.BATTLE_FIRST_MAINLOAD;
          period = 90000;
          break;
        case 1:
          error = EventErrorEnum.BATTLE_FIRST_HOLE;
          period = 30000;
          break;
        default:
          return;
      }
      slot.timing.Start(period, (TimerCallback) (callbackState =>
      {
        this.BaseCounter(error, player, slot);
        lock (callbackState)
          slot?.StopTiming();
      }));
    }

    private void BaseCounter(EventErrorEnum error, PointBlank.Game.Data.Model.Account player, PointBlank.Core.Models.Room.Slot slot)
    {
      player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_ACK(error));
      player.SendPacket((SendPacket) new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(player, 0));
      slot.state = SlotState.NORMAL;
      AllUtils.BattleEndPlayersCount(this, this.isBotMode());
      this.updateSlotsInfo();
    }

    public void StartBomb()
    {
      try
      {
        this.bomb.Start(42000, (TimerCallback) (callbackState =>
        {
          if (this != null && this.C4_actived)
          {
            ++this.red_rounds;
            this.C4_actived = false;
            AllUtils.BattleEndRound(this, 0, RoundEndType.BombFire);
          }
          lock (callbackState)
            this.bomb.Timer = (Timer) null;
        }));
      }
      catch (Exception ex)
      {
        Logger.warning("StartBomb: " + ex.ToString());
      }
    }

    public void StartVote()
    {
      try
      {
        if (this.votekick == null)
          return;
        this.vote.Start(20000, (TimerCallback) (callbackState =>
        {
          AllUtils.votekickResult(this);
          lock (callbackState)
            this.vote.Timer = (Timer) null;
        }));
      }
      catch (Exception ex)
      {
        Logger.warning("StartVote: " + ex.ToString());
        if (this.vote.Timer != null)
          this.vote.Timer = (Timer) null;
        this.votekick = (PointBlank.Core.Models.Room.VoteKick) null;
      }
    }

    public void RoundRestart()
    {
      try
      {
        this.StopBomb();
        foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
        {
          if (slot._playerId > 0L && slot.state == SlotState.BATTLE)
          {
            if (!slot._deathState.HasFlag((Enum) DeadEnum.UseChat))
              slot._deathState |= DeadEnum.UseChat;
            if (slot.espectador)
              slot.espectador = false;
            if (slot.killsOnLife >= 3 && this.room_type == RoomType.Annihilation)
              ++slot.objects;
            slot.killsOnLife = 0;
            slot.lastKillState = 0;
            slot.repeatLastState = false;
            slot.damageBar1 = (ushort) 0;
            slot.damageBar2 = (ushort) 0;
          }
        }
        this.round.Start(8000, (TimerCallback) (callbackState =>
        {
          foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
          {
            if (slot._playerId > 0L)
            {
              if (!slot._deathState.HasFlag((Enum) DeadEnum.UseChat))
                slot._deathState |= DeadEnum.UseChat;
              if (slot.espectador)
                slot.espectador = false;
            }
          }
          this.StopBomb();
          DateTime now = DateTime.Now;
          if (this._state == RoomState.Battle)
            this.BattleStart = this.room_type == RoomType.Boss || this.room_type == RoomType.CrossCounter ? now.AddSeconds(5.0) : now;
          using (PROTOCOL_BATTLE_MISSION_ROUND_PRE_START_ACK roundPreStartAck = new PROTOCOL_BATTLE_MISSION_ROUND_PRE_START_ACK(this))
          {
            using (PROTOCOL_BATTLE_MISSION_ROUND_START_ACK missionRoundStartAck = new PROTOCOL_BATTLE_MISSION_ROUND_START_ACK(this))
              this.SendPacketToPlayers((SendPacket) roundPreStartAck, (SendPacket) missionRoundStartAck, SlotState.BATTLE, 0);
          }
          this.StopBomb();
          this.swapRound = false;
          lock (callbackState)
            this.round.Timer = (Timer) null;
        }));
      }
      catch (Exception ex)
      {
        Logger.warning("[Room.RoundRestart] " + ex.ToString());
      }
    }

    public void StopBomb()
    {
      if (!this.C4_actived)
        return;
      this.C4_actived = false;
      if (this.bomb == null)
        return;
      this.bomb.Timer = (Timer) null;
    }

    public void StartBattle(bool updateInfo)
    {
      Monitor.Enter((object) this._slots);
      this._state = RoomState.Loading;
      this.requestHost.Clear();
      this.UdpServer = BattleServerXml.GetRandomServer();
      this.StartTick = DateTime.Now.Ticks;
      this.StartDate = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
      this.SetBotLevel();
      AllUtils.CheckClanMatchRestrict(this);
      using (PROTOCOL_BATTLE_START_GAME_ACK battleStartGameAck = new PROTOCOL_BATTLE_START_GAME_ACK(this))
      {
        byte[] completeBytes = battleStartGameAck.GetCompleteBytes("Room.StartBattle");
        foreach (PointBlank.Game.Data.Model.Account allPlayer in this.getAllPlayers(SlotState.READY, 0))
        {
          PointBlank.Core.Models.Room.Slot slot = this.getSlot(allPlayer._slotId);
          if (slot != null)
          {
            slot.withHost = true;
            slot.state = SlotState.LOAD;
            slot.SetMissionsClone(allPlayer._mission);
            allPlayer.SendCompletePacket(completeBytes);
          }
        }
      }
      if (updateInfo)
        this.updateSlotsInfo();
      this.updateRoomInfo();
      Monitor.Exit((object) this._slots);
    }

    public void StartCountDown()
    {
      using (PROTOCOL_BATTLE_START_COUNTDOWN_ACK startCountdownAck = new PROTOCOL_BATTLE_START_COUNTDOWN_ACK(CountDownEnum.Start))
        this.SendPacketToPlayers((SendPacket) startCountdownAck);
      this.countdown.Start(5250, (TimerCallback) (callbackState =>
      {
        try
        {
          if (this._slots[this._leader].state == SlotState.READY)
          {
            if (this._state == RoomState.CountDown)
              this.StartBattle(true);
          }
        }
        catch (Exception ex)
        {
          Logger.warning("[Room.StartCountDown] " + ex.ToString());
        }
        lock (callbackState)
          this.countdown.Timer = (Timer) null;
      }));
    }

    public void StopCountDown(CountDownEnum motive, bool refreshRoom = true)
    {
      this._state = RoomState.Ready;
      if (refreshRoom)
        this.updateRoomInfo();
      this.countdown.Timer = (Timer) null;
      using (PROTOCOL_BATTLE_START_COUNTDOWN_ACK startCountdownAck = new PROTOCOL_BATTLE_START_COUNTDOWN_ACK(motive))
        this.SendPacketToPlayers((SendPacket) startCountdownAck);
    }

    public void StopCountDown(int slotId)
    {
      if (this._state != RoomState.CountDown)
        return;
      if (slotId == this._leader)
      {
        this.StopCountDown(CountDownEnum.StopByHost, true);
      }
      else
      {
        if (this.getPlayingPlayers(this._leader % 2 == 0 ? 1 : 0, SlotState.READY, 0) != 0)
          return;
        this.changeSlotState(this._leader, SlotState.NORMAL, false);
        this.StopCountDown(CountDownEnum.StopByPlayer, true);
      }
    }

    public void CalculateResult()
    {
      lock (this._slots)
        this.BaseResultGame(AllUtils.GetWinnerTeam(this), this.isBotMode());
    }

    public void CalculateResult(TeamResultType resultType)
    {
      lock (this._slots)
        this.BaseResultGame(resultType, this.isBotMode());
    }

    public void CalculateResult(TeamResultType resultType, bool isBotMode)
    {
      lock (this._slots)
        this.BaseResultGame(resultType, isBotMode);
    }

    public void CalculateResultFreeForAll(int SlotWin)
    {
      lock (this._slots)
        this.BaseResultGameFreeForAll(SlotWin);
    }

    private void BaseResultGame(TeamResultType winnerTeam, bool isBotMode)
    {
      ServerConfig config = GameManager.Config;
      EventUpModel runningEvent1 = EventRankUpSyncer.getRunningEvent();
      EventMapModel runningEvent2 = EventMapSyncer.getRunningEvent();
      bool flag = EventMapSyncer.EventIsValid(runningEvent2, (int) this.mapId, (int) this.room_type);
      PlayTimeModel runningEvent3 = EventPlayTimeSyncer.getRunningEvent();
      DateTime now = DateTime.Now;
      if (config == null)
      {
        Logger.error("Server Config Null. RoomResult canceled.");
      }
      else
      {
        foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
        {
          PointBlank.Game.Data.Model.Account player;
          if (!slot.check && slot.state == SlotState.BATTLE && this.getPlayerBySlot(slot, out player))
          {
            DBQuery query1 = new DBQuery();
            DBQuery query2 = new DBQuery();
            slot.check = true;
            double num1 = slot.inBattleTime(now);
            int gp = player._gp;
            int exp = player._exp;
            int money = player._money;
            if (!isBotMode)
            {
              if (config.missions)
              {
                AllUtils.endMatchMission(this, player, slot, winnerTeam);
                if (slot.MissionsCompleted)
                {
                  player._mission = slot.Missions;
                  MissionManager.getInstance().updateCurrentMissionList(player.player_id, player._mission);
                }
                AllUtils.GenerateMissionAwards(player, query1);
              }
              int num2 = slot.allKills != 0 || slot.allDeaths != 0 ? (int) num1 : (int) (num1 / 3.0);
              if (this.room_type == RoomType.Bomb || this.room_type == RoomType.Annihilation)
              {
                slot.exp = (int) ((double) slot.Score + (double) num2 / 2.5 + (double) slot.allDeaths * 2.2 + (double) (slot.objects * 20));
                slot.gp = (int) ((double) slot.Score + (double) num2 / 3.0 + (double) slot.allDeaths * 2.2 + (double) (slot.objects * 20));
                slot.money = (int) ((double) (slot.Score / 2) + (double) num2 / 6.5 + (double) slot.allDeaths * 1.5 + (double) (slot.objects * 10));
              }
              else
              {
                slot.exp = (int) ((double) slot.Score + (double) num2 / 2.5 + (double) slot.allDeaths * 1.8 + (double) (slot.objects * 20));
                slot.gp = (int) ((double) slot.Score + (double) num2 / 3.0 + (double) slot.allDeaths * 1.8 + (double) (slot.objects * 20));
                slot.money = (int) ((double) slot.Score / 1.5 + (double) num2 / 4.5 + (double) slot.allDeaths * 1.1 + (double) (slot.objects * 20));
              }
              bool WonTheMatch = (TeamResultType) slot._team == winnerTeam;
              if (this.rule != 80 && this.rule != 32)
              {
                player._statistic.headshots_count += slot.headshots;
                player._statistic.kills_count += slot.allKills;
                player._statistic.totalkills_count += slot.allKills;
                player._statistic.deaths_count += slot.allDeaths;
                player._statistic.assist += slot.allAssists;
                this.AddKDInfosToQuery(slot, player._statistic, query1);
                AllUtils.updateMatchCount(WonTheMatch, player, (int) winnerTeam, query1);
                if (player.Daily != null)
                {
                  player.Daily.Kills += slot.allKills;
                  player.Daily.Deaths += slot.allDeaths;
                  player.Daily.Headshots += slot.headshots;
                  this.AddDailyToQuery(slot, player.Daily, query2);
                  AllUtils.UpdateDailyRecord(WonTheMatch, player, (int) winnerTeam, query2);
                }
              }
              if (WonTheMatch)
              {
                slot.gp += AllUtils.percentage(slot.gp, 15);
                slot.exp += AllUtils.percentage(slot.exp, 20);
              }
              if (slot.earnedXP > 0)
                slot.exp += slot.earnedXP * 5;
            }
            else
            {
              slot.gp += 300;
              slot.exp += 300;
            }
            slot.exp = slot.exp > GameConfig.maxBattleXP ? GameConfig.maxBattleXP : slot.exp;
            slot.gp = slot.gp > GameConfig.maxBattleGP ? GameConfig.maxBattleGP : slot.gp;
            slot.money = slot.money > GameConfig.maxBattleMY ? GameConfig.maxBattleMY : slot.money;
            if (slot.exp < 0 || slot.gp < 0 || slot.money < 0)
            {
              slot.exp = 2;
              slot.gp = 2;
              slot.money = 2;
            }
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int total1 = 0;
            int total2 = 0;
            if (runningEvent1 != null | flag)
            {
              if (runningEvent1 != null)
              {
                total1 += runningEvent1._percentXp;
                total2 += runningEvent1._percentGp;
              }
              if (flag)
              {
                total1 += runningEvent2._percentXp;
                total2 += runningEvent2._percentGp;
              }
              if (!slot.bonusFlags.HasFlag((Enum) ResultIcon.Event))
                slot.bonusFlags |= ResultIcon.Event;
              slot.BonusEventExp += AllUtils.percentage(total1, 100);
              slot.BonusEventPoint += AllUtils.percentage(total2, 100);
            }
            PlayerBonus bonus = player._bonus;
            if (bonus != null && bonus.bonuses > 0)
            {
              if ((bonus.bonuses & 8) == 8)
                num3 += 100;
              if ((bonus.bonuses & 128) == 128)
                num4 += 100;
              if ((bonus.bonuses & 4) == 4)
                num3 += 50;
              if ((bonus.bonuses & 64) == 64)
                num4 += 50;
              if ((bonus.bonuses & 2) == 2)
                num3 += 30;
              if ((bonus.bonuses & 32) == 32)
                num4 += 30;
              if ((bonus.bonuses & 1) == 1)
                num3 += 10;
              if ((bonus.bonuses & 16) == 16)
                num4 += 10;
              if (!slot.bonusFlags.HasFlag((Enum) ResultIcon.Item))
                slot.bonusFlags |= ResultIcon.Item;
              slot.BonusItemExp += num3;
              slot.BonusItemPoint += num4;
            }
            if (player.pc_cafe == 2 || player.pc_cafe == 1)
            {
              int num2 = num5 + (player.pc_cafe == 2 ? 120 : GameConfig.ICafeExp);
              int num7 = num6 + (player.pc_cafe == 2 ? 100 : GameConfig.ICafePoint);
              if (player.pc_cafe == 1 && !slot.bonusFlags.HasFlag((Enum) ResultIcon.Pc))
                slot.bonusFlags |= ResultIcon.Pc;
              else if (player.pc_cafe == 2 && !slot.bonusFlags.HasFlag((Enum) ResultIcon.PcPlus))
                slot.bonusFlags |= ResultIcon.PcPlus;
              slot.BonusCafePoint += num7;
              slot.BonusCafeExp += num2;
            }
            if (isBotMode)
            {
              if (slot.BonusItemExp > 300)
                slot.BonusItemExp = 300;
              if (slot.BonusItemPoint > 300)
                slot.BonusItemPoint = 300;
              if (slot.BonusCafeExp > 300)
                slot.BonusCafeExp = 300;
              if (slot.BonusCafePoint > 300)
                slot.BonusCafePoint = 300;
              if (slot.BonusEventExp > 300)
                slot.BonusEventExp = 300;
              if (slot.BonusEventPoint > 300)
                slot.BonusEventPoint = 300;
            }
            player._gp += slot.gp + slot.BonusItemPoint + slot.BonusEventPoint + slot.BonusCafePoint;
            player._exp += slot.exp + slot.BonusItemExp + slot.BonusEventExp + slot.BonusCafeExp;
            if (player.Daily != null)
            {
              player.Daily.Point += slot.gp + slot.BonusItemPoint + slot.BonusEventPoint + slot.BonusCafePoint;
              player.Daily.Exp += slot.exp + slot.BonusItemExp + slot.BonusEventExp + slot.BonusCafeExp;
              query2.AddQuery("point", (object) player.Daily.Point);
              query2.AddQuery("exp", (object) player.Daily.Exp);
            }
            if (GameConfig.winCashPerBattle)
              player._money += slot.money;
            RankModel rank = RankXml.getRank(player._rank);
            if (rank != null && player._exp >= rank._onNextLevel + rank._onAllExp && player._rank <= 50)
            {
              List<ItemsModel> awards = RankXml.getAwards(player._rank);
              if (awards.Count > 0)
              {
                for (int index = 0; index < awards.Count; ++index)
                {
                  ItemsModel itemsModel = awards[index];
                  if (itemsModel._id != 0)
                    player.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, itemsModel));
                }
              }
              player._gp += rank._onGPUp;
              player.LastRankUpDate = uint.Parse(now.ToString("yyMMddHHmm"));
              player.SendPacket((SendPacket) new PROTOCOL_BASE_RANK_UP_ACK(++player._rank, rank._onNextLevel));
              query1.AddQuery("last_rankup_date", (object) (long) player.LastRankUpDate);
              query1.AddQuery("rank", (object) player._rank);
            }
            if (runningEvent3 != null)
              AllUtils.PlayTimeEvent((long) num1, player, runningEvent3, isBotMode);
            AllUtils.DiscountPlayerItems(slot, player);
            if (gp != player._gp)
              query1.AddQuery("gp", (object) player._gp);
            if (exp != player._exp)
              query1.AddQuery("exp", (object) player._exp);
            if (money != player._money)
              query1.AddQuery("money", (object) player._money);
            ComDiv.updateDB("accounts", "player_id", (object) player.player_id, query1.GetTables(), query1.GetValues());
            ComDiv.updateDB("player_dailyrecord", "player_id", (object) player.player_id, query2.GetTables(), query2.GetValues());
            if (GameConfig.winCashPerBattle && GameConfig.showCashReceiveWarn)
              player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(Translation.GetLabel("CashReceived", (object) slot.money)));
          }
        }
        this.updateSlotsInfo();
        this.CalculateClanMatchResult((int) winnerTeam);
      }
    }

    private void BaseResultGameFreeForAll(int winner)
    {
      ServerConfig config = GameManager.Config;
      EventUpModel runningEvent1 = EventRankUpSyncer.getRunningEvent();
      EventMapModel runningEvent2 = EventMapSyncer.getRunningEvent();
      bool flag = EventMapSyncer.EventIsValid(runningEvent2, (int) this.mapId, (int) this.room_type);
      PlayTimeModel runningEvent3 = EventPlayTimeSyncer.getRunningEvent();
      DateTime now = DateTime.Now;
      int[] numArray = new int[16];
      int SlotWin = 0;
      if (config == null)
      {
        Logger.error("Server Config Null. RoomResult canceled.");
      }
      else
      {
        for (int index = 0; index < 16; ++index)
        {
          PointBlank.Core.Models.Room.Slot slot = this._slots[index];
          numArray[index] = slot._playerId == 0L ? 0 : slot.allKills;
          if (numArray[index] > numArray[SlotWin])
            SlotWin = index;
          PointBlank.Game.Data.Model.Account player;
          if (!slot.check && slot.state == SlotState.BATTLE && this.getPlayerBySlot(slot, out player))
          {
            DBQuery query1 = new DBQuery();
            DBQuery query2 = new DBQuery();
            slot.check = true;
            double num1 = slot.inBattleTime(now);
            int gp = player._gp;
            int exp = player._exp;
            int money = player._money;
            if (config.missions)
            {
              AllUtils.endMatchMission(this, player, slot, winner == SlotWin ? (this._slots[SlotWin]._team == 0 ? TeamResultType.TeamRedWin : TeamResultType.TeamBlueWin) : TeamResultType.TeamDraw);
              if (slot.MissionsCompleted)
              {
                player._mission = slot.Missions;
                MissionManager.getInstance().updateCurrentMissionList(player.player_id, player._mission);
              }
              AllUtils.GenerateMissionAwards(player, query1);
            }
            int num2 = slot.allKills != 0 || slot.allDeaths != 0 ? (int) num1 : (int) (num1 / 3.0);
            slot.exp = (int) ((double) slot.Score + (double) num2 / 2.5 + (double) slot.allDeaths * 1.8 + (double) (slot.objects * 20));
            slot.gp = (int) ((double) slot.Score + (double) num2 / 3.0 + (double) slot.allDeaths * 1.8 + (double) (slot.objects * 20));
            slot.money = (int) ((double) slot.Score / 1.5 + (double) num2 / 4.5 + (double) slot.allDeaths * 1.1 + (double) (slot.objects * 20));
            if (this.rule != 80 && this.rule != 32)
            {
              player._statistic.headshots_count += slot.headshots;
              player._statistic.kills_count += slot.allKills;
              player._statistic.totalkills_count += slot.allKills;
              player._statistic.deaths_count += slot.allDeaths;
              player._statistic.assist += slot.allAssists;
              this.AddKDInfosToQuery(slot, player._statistic, query1);
              AllUtils.updateMatchCountFreeForAll(this, player, SlotWin, query1);
              if (player.Daily != null)
              {
                player.Daily.Kills += slot.allKills;
                player.Daily.Deaths += slot.allDeaths;
                player.Daily.Headshots += slot.headshots;
                this.AddDailyToQuery(slot, player.Daily, query2);
                AllUtils.UpdateMatchDailyRecordFreeForAll(this, player, SlotWin, query2);
              }
            }
            if (winner == SlotWin)
            {
              slot.gp += AllUtils.percentage(slot.gp, 15);
              slot.exp += AllUtils.percentage(slot.exp, 20);
            }
            if (slot.earnedXP > 0)
              slot.exp += slot.earnedXP * 5;
            slot.exp = slot.exp > GameConfig.maxBattleXP ? GameConfig.maxBattleXP : slot.exp;
            slot.gp = slot.gp > GameConfig.maxBattleGP ? GameConfig.maxBattleGP : slot.gp;
            slot.money = slot.money > GameConfig.maxBattleMY ? GameConfig.maxBattleMY : slot.money;
            if (slot.exp < 0 || slot.gp < 0 || slot.money < 0)
            {
              slot.exp = 2;
              slot.gp = 2;
              slot.money = 2;
            }
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int total1 = 0;
            int total2 = 0;
            if (runningEvent1 != null | flag)
            {
              if (runningEvent1 != null)
              {
                total1 += runningEvent1._percentXp;
                total2 += runningEvent1._percentGp;
              }
              if (flag)
              {
                total1 += runningEvent2._percentXp;
                total2 += runningEvent2._percentGp;
              }
              if (!slot.bonusFlags.HasFlag((Enum) ResultIcon.Event))
                slot.bonusFlags |= ResultIcon.Event;
              slot.BonusEventExp += AllUtils.percentage(total1, 100);
              slot.BonusEventPoint += AllUtils.percentage(total2, 100);
            }
            PlayerBonus bonus = player._bonus;
            if (bonus != null && bonus.bonuses > 0)
            {
              if ((bonus.bonuses & 8) == 8)
                num3 += 100;
              if ((bonus.bonuses & 128) == 128)
                num4 += 100;
              if ((bonus.bonuses & 4) == 4)
                num3 += 50;
              if ((bonus.bonuses & 64) == 64)
                num4 += 50;
              if ((bonus.bonuses & 2) == 2)
                num3 += 30;
              if ((bonus.bonuses & 32) == 32)
                num4 += 30;
              if ((bonus.bonuses & 1) == 1)
                num3 += 10;
              if ((bonus.bonuses & 16) == 16)
                num4 += 10;
              if (!slot.bonusFlags.HasFlag((Enum) ResultIcon.Item))
                slot.bonusFlags |= ResultIcon.Item;
              slot.BonusItemExp += num3;
              slot.BonusItemPoint += num4;
            }
            if (player.pc_cafe == 2 || player.pc_cafe == 1)
            {
              int num7 = num5 + (player.pc_cafe == 2 ? 120 : GameConfig.ICafeExp);
              int num8 = num6 + (player.pc_cafe == 2 ? 100 : GameConfig.ICafePoint);
              if (player.pc_cafe == 1 && !slot.bonusFlags.HasFlag((Enum) ResultIcon.Pc))
                slot.bonusFlags |= ResultIcon.Pc;
              else if (player.pc_cafe == 2 && !slot.bonusFlags.HasFlag((Enum) ResultIcon.PcPlus))
                slot.bonusFlags |= ResultIcon.PcPlus;
              slot.BonusCafePoint += num8;
              slot.BonusCafeExp += num7;
            }
            player._gp += slot.gp + slot.BonusItemPoint + slot.BonusEventPoint + slot.BonusCafePoint;
            player._exp += slot.exp + slot.BonusItemExp + slot.BonusEventExp + slot.BonusCafeExp;
            if (player.Daily != null)
            {
              player.Daily.Point += slot.gp + slot.BonusItemPoint + slot.BonusEventPoint + slot.BonusCafePoint;
              player.Daily.Exp += slot.exp + slot.BonusItemExp + slot.BonusEventExp + slot.BonusCafeExp;
              query2.AddQuery("point", (object) player.Daily.Point);
              query2.AddQuery("exp", (object) player.Daily.Exp);
            }
            if (GameConfig.winCashPerBattle)
              player._money += slot.money;
            RankModel rank = RankXml.getRank(player._rank);
            if (rank != null && player._exp >= rank._onNextLevel + rank._onAllExp && player._rank <= 50)
            {
              List<ItemsModel> awards = RankXml.getAwards(player._rank);
              if (awards.Count > 0)
              {
                foreach (ItemsModel itemsModel in awards)
                {
                  if (itemsModel._id != 0)
                    player.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, itemsModel));
                }
              }
              player._gp += rank._onGPUp;
              player.LastRankUpDate = uint.Parse(now.ToString("yyMMddHHmm"));
              player.SendPacket((SendPacket) new PROTOCOL_BASE_RANK_UP_ACK(++player._rank, rank._onNextLevel));
              query1.AddQuery("last_rankup_date", (object) (long) player.LastRankUpDate);
              query1.AddQuery("rank", (object) player._rank);
            }
            if (runningEvent3 != null)
              AllUtils.PlayTimeEvent((long) num1, player, runningEvent3, false);
            AllUtils.DiscountPlayerItems(slot, player);
            if (gp != player._gp)
              query1.AddQuery("gp", (object) player._gp);
            if (exp != player._exp)
              query1.AddQuery("exp", (object) player._exp);
            if (money != player._money)
              query1.AddQuery("money", (object) player._money);
            ComDiv.updateDB("accounts", "player_id", (object) player.player_id, query1.GetTables(), query1.GetValues());
            ComDiv.updateDB("player_dailyrecord", "player_id", (object) player.player_id, query2.GetTables(), query2.GetValues());
            if (GameConfig.winCashPerBattle && GameConfig.showCashReceiveWarn)
              player.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(Translation.GetLabel("CashReceived", (object) slot.money)));
          }
        }
        this.updateSlotsInfo();
      }
    }

    private void AddKDInfosToQuery(PointBlank.Core.Models.Room.Slot slot, PlayerStats stats, DBQuery query)
    {
      if (slot.allKills > 0)
      {
        query.AddQuery("kills_count", (object) stats.kills_count);
        query.AddQuery("totalkills_count", (object) stats.totalkills_count);
      }
      if (slot.allAssists > 0)
        query.AddQuery("assist", (object) stats.assist);
      if (slot.allDeaths > 0)
        query.AddQuery("deaths_count", (object) stats.deaths_count);
      if (slot.headshots <= 0)
        return;
      query.AddQuery("headshots_count", (object) stats.headshots_count);
    }

    private void AddDailyToQuery(PointBlank.Core.Models.Room.Slot slot, PlayerDailyRecord Daily, DBQuery query)
    {
      if (Daily.Kills > 0)
        query.AddQuery("kills", (object) Daily.Kills);
      if (Daily.Deaths > 0)
        query.AddQuery("deaths", (object) Daily.Deaths);
      if (Daily.Headshots <= 0)
        return;
      query.AddQuery("headshots", (object) Daily.Headshots);
    }

    private void CalculateClanMatchResult(int winnerTeam)
    {
      if (this._channelType != 4 || this.blockedClan)
        return;
      SortedList<int, PointBlank.Core.Models.Account.Clan.Clan> sortedList = new SortedList<int, PointBlank.Core.Models.Account.Clan.Clan>();
      foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
      {
        PointBlank.Game.Data.Model.Account player;
        if (slot.state == SlotState.BATTLE && this.getPlayerBySlot(slot, out player))
        {
          PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
          if (clan._id != 0)
          {
            bool WonTheMatch = slot._team == winnerTeam;
            clan._exp += slot.exp;
            clan.BestPlayers.SetBestExp(slot);
            clan.BestPlayers.SetBestKills(slot);
            clan.BestPlayers.SetBestHeadshot(slot);
            clan.BestPlayers.SetBestWins(player._statistic, slot, WonTheMatch);
            clan.BestPlayers.SetBestParticipation(player._statistic, slot);
            if (!sortedList.ContainsKey(player.clanId))
            {
              sortedList.Add(player.clanId, clan);
              if (winnerTeam != 2)
              {
                this.CalculateSpecialCM(clan, winnerTeam, slot._team);
                if (WonTheMatch)
                  ++clan.vitorias;
                else
                  ++clan.derrotas;
              }
              PlayerManager.updateClanBattles(clan._id, ++clan.partidas, clan.vitorias, clan.derrotas);
            }
          }
        }
      }
      foreach (PointBlank.Core.Models.Account.Clan.Clan clan in (IEnumerable<PointBlank.Core.Models.Account.Clan.Clan>) sortedList.Values)
      {
        PlayerManager.updateClanExp(clan._id, clan._exp);
        PlayerManager.updateClanPoints(clan._id, clan._pontos);
        PlayerManager.updateBestPlayers(clan);
        RankModel rank = ClanRankXml.getRank(clan._rank);
        if (rank != null && clan._exp >= rank._onNextLevel + rank._onAllExp)
          PlayerManager.updateClanRank(clan._id, ++clan._rank);
      }
    }

    private void CalculateSpecialCM(PointBlank.Core.Models.Account.Clan.Clan clan, int winnerTeam, int teamIdx)
    {
      if (winnerTeam == 2)
        return;
      if (winnerTeam == teamIdx)
      {
        float num = 25f + (this.room_type != RoomType.DeathMatch ? (teamIdx == 0 ? (float) this.red_rounds : (float) this.blue_rounds) : (float) ((teamIdx == 0 ? this._redKills : this._blueKills) / 20));
        clan._pontos += num;
      }
      else
      {
        if ((double) clan._pontos == 0.0)
          return;
        float num = 40f - (this.room_type != RoomType.DeathMatch ? (teamIdx == 0 ? (float) this.red_rounds : (float) this.blue_rounds) : (float) ((teamIdx == 0 ? this._redKills : this._blueKills) / 20));
        clan._pontos -= num;
      }
    }

    public bool isStartingMatch()
    {
      return this._state > RoomState.Ready;
    }

    public bool isPreparing()
    {
      return this._state >= RoomState.Loading;
    }

    public void updateRoomInfo()
    {
      this.SetSeed();
      using (PROTOCOL_ROOM_CHANGE_ROOMINFO_ACK changeRoominfoAck = new PROTOCOL_ROOM_CHANGE_ROOMINFO_ACK(this))
        this.SendPacketToPlayers((SendPacket) changeRoominfoAck);
    }

    public void initSlotCount(int count, bool Change = false)
    {
      MapMatch mapMatch = MapModel.Matchs.Find((Predicate<MapMatch>) (x =>
      {
        if ((MapIdEnum) x.Id == this.mapId)
          return MapModel.getRule(x.Mode).Rule == this.rule;
        return false;
      }));
      if (mapMatch != null)
        count = mapMatch.Limit;
      if (this.room_type == RoomType.Tutorial)
        count = 1;
      if (this.isBotMode())
        count = 8;
      if (count <= 0)
        count = 1;
      for (int index = 0; index < this._slots.Length; ++index)
      {
        if (index >= count)
          this._slots[index].state = SlotState.CLOSE;
      }
      if (!Change)
        return;
      this.updateSlotsInfo();
    }

    public int getSlotCount()
    {
      lock (this._slots)
      {
        int num = 0;
        for (int index = 0; index < this._slots.Length; ++index)
        {
          if (this._slots[index].state != SlotState.CLOSE)
            ++num;
        }
        return num;
      }
    }

    public void SwitchNewSlot(
      List<SlotChange> slots,
      PointBlank.Game.Data.Model.Account p,
      PointBlank.Core.Models.Room.Slot old,
      int teamIdx,
      bool Mode)
    {
      if (Mode)
      {
        PointBlank.Core.Models.Room.Slot slot = this._slots[teamIdx];
        if (slot._playerId != 0L || slot.state != SlotState.EMPTY)
          return;
        slot.state = SlotState.NORMAL;
        slot._playerId = p.player_id;
        slot._equip = p._equip;
        old.state = SlotState.EMPTY;
        old._playerId = 0L;
        old._equip = (PlayerEquipedItems) null;
        if (p._slotId == this._leader)
          this._leader = teamIdx;
        p._slotId = teamIdx;
        slots.Add(new SlotChange()
        {
          oldSlot = old,
          newSlot = slot
        });
      }
      else
      {
        for (int index = 0; index < this.GetTeamArray(teamIdx).Length; ++index)
        {
          int team = this.GetTeamArray(teamIdx)[index];
          PointBlank.Core.Models.Room.Slot slot = this._slots[team];
          if (slot._playerId == 0L && slot.state == SlotState.EMPTY)
          {
            slot.state = SlotState.NORMAL;
            slot._playerId = p.player_id;
            slot._equip = p._equip;
            old.state = SlotState.EMPTY;
            old._playerId = 0L;
            old._equip = (PlayerEquipedItems) null;
            if (p._slotId == this._leader)
              this._leader = team;
            p._slotId = team;
            slots.Add(new SlotChange()
            {
              oldSlot = old,
              newSlot = slot
            });
            break;
          }
        }
      }
    }

    public void SwitchSlots(
      List<SlotChange> slots,
      int newSlotId,
      int oldSlotId,
      bool changeReady)
    {
      PointBlank.Core.Models.Room.Slot slot1 = this._slots[newSlotId];
      PointBlank.Core.Models.Room.Slot slot2 = this._slots[oldSlotId];
      if (changeReady)
      {
        if (slot1.state == SlotState.READY)
          slot1.state = SlotState.NORMAL;
        if (slot2.state == SlotState.READY)
          slot2.state = SlotState.NORMAL;
      }
      slot1.SetSlotId(oldSlotId);
      slot2.SetSlotId(newSlotId);
      this._slots[newSlotId] = slot2;
      this._slots[oldSlotId] = slot1;
      slots.Add(new SlotChange()
      {
        oldSlot = slot1,
        newSlot = slot2
      });
    }

    public void changeSlotState(int slotId, SlotState state, bool sendInfo)
    {
      this.changeSlotState(this.getSlot(slotId), state, sendInfo);
    }

    public void changeSlotState(PointBlank.Core.Models.Room.Slot slot, SlotState state, bool sendInfo)
    {
      if (slot == null || slot.state == state)
        return;
      slot.state = state;
      if (state == SlotState.EMPTY || state == SlotState.CLOSE)
      {
        AllUtils.ResetSlotInfo(this, slot, false);
        slot._playerId = 0L;
      }
      if (!sendInfo)
        return;
      this.updateSlotsInfo();
    }

    public PointBlank.Game.Data.Model.Account getPlayerBySlot(PointBlank.Core.Models.Room.Slot slot)
    {
      try
      {
        long playerId = slot._playerId;
        return playerId > 0L ? AccountManager.getAccount(playerId, true) : (PointBlank.Game.Data.Model.Account) null;
      }
      catch
      {
        return (PointBlank.Game.Data.Model.Account) null;
      }
    }

    public PointBlank.Game.Data.Model.Account getPlayerBySlot(int slotId)
    {
      try
      {
        long playerId = this._slots[slotId]._playerId;
        return playerId > 0L ? AccountManager.getAccount(playerId, true) : (PointBlank.Game.Data.Model.Account) null;
      }
      catch
      {
        return (PointBlank.Game.Data.Model.Account) null;
      }
    }

    public bool getPlayerBySlot(int slotId, out PointBlank.Game.Data.Model.Account player)
    {
      try
      {
        long playerId = this._slots[slotId]._playerId;
        player = playerId > 0L ? AccountManager.getAccount(playerId, true) : (PointBlank.Game.Data.Model.Account) null;
        return player != null;
      }
      catch
      {
        player = (PointBlank.Game.Data.Model.Account) null;
        return false;
      }
    }

    public bool getPlayerBySlot(PointBlank.Core.Models.Room.Slot slot, out PointBlank.Game.Data.Model.Account player)
    {
      try
      {
        long playerId = slot._playerId;
        player = playerId > 0L ? AccountManager.getAccount(playerId, true) : (PointBlank.Game.Data.Model.Account) null;
        return player != null;
      }
      catch
      {
        player = (PointBlank.Game.Data.Model.Account) null;
        return false;
      }
    }

    public int getTimeByMask()
    {
      return this.TIMES[this.killtime >> 4];
    }

    public int getRoundsByMask()
    {
      return this.ROUNDS[this.killtime & 15];
    }

    public int getKillsByMask()
    {
      return this.KILLS[this.killtime & 15];
    }

    public void updateSlotsInfo()
    {
      using (PROTOCOL_ROOM_GET_SLOTINFO_ACK roomGetSlotinfoAck = new PROTOCOL_ROOM_GET_SLOTINFO_ACK(this))
        this.SendPacketToPlayers((SendPacket) roomGetSlotinfoAck);
    }

    public bool getLeader(out PointBlank.Game.Data.Model.Account p)
    {
      p = (PointBlank.Game.Data.Model.Account) null;
      if (this.getAllPlayers().Count <= 0)
        return false;
      if (this._leader == -1)
        this.setNewLeader(-1, 0, -1, false);
      if (this._leader >= 0)
        p = AccountManager.getAccount(this._slots[this._leader]._playerId, true);
      return p != null;
    }

    public PointBlank.Game.Data.Model.Account getLeader()
    {
      if (this.getAllPlayers().Count <= 0)
        return (PointBlank.Game.Data.Model.Account) null;
      if (this._leader == -1)
        this.setNewLeader(-1, 0, -1, false);
      if (this._leader != -1)
        return AccountManager.getAccount(this._slots[this._leader]._playerId, true);
      return (PointBlank.Game.Data.Model.Account) null;
    }

    public void setNewLeader(int leader, int state, int oldLeader, bool updateInfo)
    {
      Monitor.Enter((object) this._slots);
      if (leader == -1)
      {
        for (int index = 0; index < 16; ++index)
        {
          PointBlank.Core.Models.Room.Slot slot = this._slots[index];
          if (index != oldLeader && slot._playerId > 0L && slot.state > (SlotState) state)
          {
            this._leader = index;
            break;
          }
        }
      }
      else
        this._leader = leader;
      if (this._leader != -1)
      {
        PointBlank.Core.Models.Room.Slot slot = this._slots[this._leader];
        if (slot.state == SlotState.READY)
          slot.state = SlotState.NORMAL;
        if (updateInfo)
          this.updateSlotsInfo();
      }
      Monitor.Exit((object) this._slots);
    }

    public void SendPacketToPlayers(SendPacket packet)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = this.getAllPlayers();
      if (allPlayers.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket)");
      foreach (PointBlank.Game.Data.Model.Account account in allPlayers)
        account.SendCompletePacket(completeBytes);
    }

    public void SendPacketToPlayers(SendPacket packet, long player_id)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = this.getAllPlayers(player_id);
      if (allPlayers.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,long)");
      foreach (PointBlank.Game.Data.Model.Account account in allPlayers)
        account.SendCompletePacket(completeBytes);
    }

    public void SendPacketToPlayers(SendPacket packet, SlotState state, int type)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = this.getAllPlayers(state, type);
      if (allPlayers.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SLOT_STATE,int)");
      for (int index = 0; index < allPlayers.Count; ++index)
        allPlayers[index].SendCompletePacket(completeBytes);
    }

    public void SendPacketToPlayers(
      SendPacket packet,
      SendPacket packet2,
      SlotState state,
      int type)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = this.getAllPlayers(state, type);
      if (allPlayers.Count == 0)
        return;
      byte[] completeBytes1 = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SendPacket,SLOT_STATE,int)-1");
      byte[] completeBytes2 = packet2.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SendPacket,SLOT_STATE,int)-2");
      foreach (PointBlank.Game.Data.Model.Account account in allPlayers)
      {
        account.SendCompletePacket(completeBytes1);
        account.SendCompletePacket(completeBytes2);
      }
    }

    public void SendPacketToPlayers(SendPacket packet, SlotState state, int type, int exception)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = this.getAllPlayers(state, type, exception);
      if (allPlayers.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SLOT_STATE,int,int)");
      foreach (PointBlank.Game.Data.Model.Account account in allPlayers)
        account.SendCompletePacket(completeBytes);
    }

    public void SendPacketToPlayers(
      SendPacket packet,
      SlotState state,
      int type,
      int exception,
      int exception2)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = this.getAllPlayers(state, type, exception, exception2);
      if (allPlayers.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SLOT_STATE,int,int,int)");
      foreach (PointBlank.Game.Data.Model.Account account in allPlayers)
        account.SendCompletePacket(completeBytes);
    }

    public void RemovePlayer(PointBlank.Game.Data.Model.Account player, bool WarnAllPlayers, int quitMotive = 0)
    {
      PointBlank.Core.Models.Room.Slot slot;
      if (player == null || !this.getSlot(player._slotId, out slot))
        return;
      this.BaseRemovePlayer(player, slot, WarnAllPlayers, quitMotive);
    }

    public void RemovePlayer(PointBlank.Game.Data.Model.Account player, PointBlank.Core.Models.Room.Slot slot, bool WarnAllPlayers, int quitMotive = 0)
    {
      if (player == null || slot == null)
        return;
      this.BaseRemovePlayer(player, slot, WarnAllPlayers, quitMotive);
    }

    private void BaseRemovePlayer(PointBlank.Game.Data.Model.Account player, PointBlank.Core.Models.Room.Slot slot, bool WarnAllPlayers, int quitMotive)
    {
      Monitor.Enter((object) this._slots);
      bool flag = false;
      bool host = false;
      if (player != null && slot != null)
      {
        if (slot.state >= SlotState.LOAD)
        {
          if (this._leader == slot._id)
          {
            int leader = this._leader;
            int state = 1;
            if (this._state == RoomState.Battle)
              state = 14;
            else if (this._state >= RoomState.Loading)
              state = 9;
            if (this.getAllPlayers(slot._id).Count >= 1)
              this.setNewLeader(-1, state, this._leader, false);
            if (this.getPlayingPlayers(2, SlotState.READY, 1) >= 2)
            {
              using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK leaveP2PserverAck = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(this))
                this.SendPacketToPlayers((SendPacket) leaveP2PserverAck, SlotState.RENDEZVOUS, 1, slot._id);
            }
            host = true;
          }
          using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK battleGiveupbattleAck = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(player, quitMotive))
            this.SendPacketToPlayers((SendPacket) battleGiveupbattleAck, SlotState.READY, 1, !WarnAllPlayers ? slot._id : -1);
          BattleLeaveSync.SendUDPPlayerLeave(this, slot._id);
          slot.ResetSlot();
          if (this.votekick != null)
            this.votekick.TotalArray[slot._id] = false;
        }
        slot._playerId = 0L;
        slot._equip = (PlayerEquipedItems) null;
        slot.state = SlotState.EMPTY;
        if (this._state == RoomState.CountDown)
        {
          if (slot._id == this._leader)
          {
            this._state = RoomState.Ready;
            flag = true;
            this.countdown.Timer = (Timer) null;
            using (PROTOCOL_BATTLE_START_COUNTDOWN_ACK startCountdownAck = new PROTOCOL_BATTLE_START_COUNTDOWN_ACK(CountDownEnum.StopByHost))
              this.SendPacketToPlayers((SendPacket) startCountdownAck);
          }
          else if (this.getPlayingPlayers(slot._team, SlotState.READY, 0) == 0)
          {
            if (slot._id != this._leader)
              this.changeSlotState(this._leader, SlotState.NORMAL, false);
            this.StopCountDown(CountDownEnum.StopByPlayer, false);
            flag = true;
          }
        }
        else if (this.isPreparing())
        {
          AllUtils.BattleEndPlayersCount(this, this.isBotMode());
          if (this._state == RoomState.Battle)
            AllUtils.BattleEndRoundPlayersCount(this);
        }
        this.CheckToEndWaitingBattle(host);
        this.requestHost.Remove(player.player_id);
        if (this.vote.Timer != null && this.votekick != null && (this.votekick.victimIdx == player._slotId && quitMotive != 2))
        {
          this.vote.Timer = (Timer) null;
          this.votekick = (PointBlank.Core.Models.Room.VoteKick) null;
          using (PROTOCOL_BATTLE_NOTIFY_KICKVOTE_CANCEL_ACK kickvoteCancelAck = new PROTOCOL_BATTLE_NOTIFY_KICKVOTE_CANCEL_ACK())
            this.SendPacketToPlayers((SendPacket) kickvoteCancelAck, SlotState.BATTLE, 0);
        }
        Match match = player._match;
        if (match != null && player.matchSlot >= 0)
        {
          match._slots[player.matchSlot].state = SlotMatchState.Normal;
          using (PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK registMercenaryAck = new PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK(match))
            match.SendPacketToPlayers((SendPacket) registMercenaryAck);
        }
        player._room = (PointBlank.Game.Data.Model.Room) null;
        player._slotId = -1;
        player._status.updateRoom(byte.MaxValue);
        AllUtils.syncPlayerToClanMembers(player);
        AllUtils.syncPlayerToFriends(player, false);
        player.updateCacheInfo();
      }
      this.updateSlotsInfo();
      if (flag)
        this.updateRoomInfo();
      Monitor.Exit((object) this._slots);
    }

    public int addPlayer(PointBlank.Game.Data.Model.Account p)
    {
      lock (this._slots)
      {
        for (int index = 0; index < 16; ++index)
        {
          PointBlank.Core.Models.Room.Slot slot = this._slots[index];
          if (slot._playerId == 0L && slot.state == SlotState.EMPTY)
          {
            slot._playerId = p.player_id;
            slot.state = SlotState.NORMAL;
            p._room = this;
            p._slotId = index;
            slot._equip = p._equip;
            p._status.updateRoom((byte) this._roomId);
            AllUtils.syncPlayerToClanMembers(p);
            AllUtils.syncPlayerToFriends(p, false);
            p.updateCacheInfo();
            return index;
          }
        }
      }
      return -1;
    }

    public int addPlayer(PointBlank.Game.Data.Model.Account p, int teamIdx)
    {
      int[] teamArray = this.GetTeamArray(teamIdx);
      lock (this._slots)
      {
        for (int index1 = 0; index1 < teamArray.Length; ++index1)
        {
          int index2 = teamArray[index1];
          PointBlank.Core.Models.Room.Slot slot = this._slots[index2];
          if (slot._playerId == 0L && slot.state == SlotState.EMPTY)
          {
            slot._playerId = p.player_id;
            slot.state = SlotState.NORMAL;
            p._room = this;
            p._slotId = index2;
            slot._equip = p._equip;
            p._status.updateRoom((byte) this._roomId);
            AllUtils.syncPlayerToClanMembers(p);
            AllUtils.syncPlayerToFriends(p, false);
            p.updateCacheInfo();
            return index2;
          }
        }
      }
      return -1;
    }

    public int[] GetTeamArray(int index)
    {
      if (index != 0)
        return this.BLUE_TEAM;
      return this.RED_TEAM;
    }

    public List<PointBlank.Game.Data.Model.Account> getAllPlayers(
      SlotState state,
      int type)
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      lock (this._slots)
      {
        for (int index = 0; index < this._slots.Length; ++index)
        {
          PointBlank.Core.Models.Room.Slot slot = this._slots[index];
          long playerId = slot._playerId;
          if (playerId > 0L && (type == 0 && slot.state == state || type == 1 && slot.state > state))
          {
            PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(playerId, true);
            if (account != null && account._slotId != -1)
              accountList.Add(account);
          }
        }
      }
      return accountList;
    }

    public List<PointBlank.Game.Data.Model.Account> getAllPlayers(
      SlotState state,
      int type,
      int exception)
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      lock (this._slots)
      {
        for (int index = 0; index < this._slots.Length; ++index)
        {
          PointBlank.Core.Models.Room.Slot slot = this._slots[index];
          long playerId = slot._playerId;
          if (playerId > 0L && index != exception && (type == 0 && slot.state == state || type == 1 && slot.state > state))
          {
            PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(playerId, true);
            if (account != null && account._slotId != -1)
              accountList.Add(account);
          }
        }
      }
      return accountList;
    }

    public List<PointBlank.Game.Data.Model.Account> getAllPlayers(
      SlotState state,
      int type,
      int exception,
      int exception2)
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      lock (this._slots)
      {
        for (int index = 0; index < this._slots.Length; ++index)
        {
          PointBlank.Core.Models.Room.Slot slot = this._slots[index];
          long playerId = slot._playerId;
          if (playerId > 0L && index != exception && index != exception2 && (type == 0 && slot.state == state || type == 1 && slot.state > state))
          {
            PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(playerId, true);
            if (account != null && account._slotId != -1)
              accountList.Add(account);
          }
        }
      }
      return accountList;
    }

    public List<PointBlank.Game.Data.Model.Account> getAllPlayers(int exception)
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      lock (this._slots)
      {
        for (int index = 0; index < this._slots.Length; ++index)
        {
          long playerId = this._slots[index]._playerId;
          if (playerId > 0L && index != exception)
          {
            PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(playerId, true);
            if (account != null && account._slotId != -1)
              accountList.Add(account);
          }
        }
      }
      return accountList;
    }

    public List<PointBlank.Game.Data.Model.Account> getAllPlayers(long exception)
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      lock (this._slots)
      {
        for (int index = 0; index < this._slots.Length; ++index)
        {
          long playerId = this._slots[index]._playerId;
          if (playerId > 0L && playerId != exception)
          {
            PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(playerId, true);
            if (account != null && account._slotId != -1)
              accountList.Add(account);
          }
        }
      }
      return accountList;
    }

    public List<PointBlank.Game.Data.Model.Account> getAllPlayers()
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      lock (this._slots)
      {
        for (int index = 0; index < this._slots.Length; ++index)
        {
          long playerId = this._slots[index]._playerId;
          if (playerId > 0L)
          {
            PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(playerId, true);
            if (account != null && account._slotId != -1)
              accountList.Add(account);
          }
        }
      }
      return accountList;
    }

    public int getPlayingPlayers(int team, bool inBattle)
    {
      int num = 0;
      lock (this._slots)
      {
        foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
        {
          if (slot._playerId > 0L && (slot._team == team || team == 2) && (inBattle && slot.state == SlotState.BATTLE_LOAD && !slot.espectador || !inBattle && slot.state >= SlotState.LOAD))
            ++num;
        }
      }
      return num;
    }

    public int getPlayingPlayers(int team, SlotState state, int type)
    {
      int num = 0;
      lock (this._slots)
      {
        foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
        {
          if (slot._playerId > 0L && (type == 0 && slot.state == state || type == 1 && slot.state > state) && (team == 2 || slot._team == team))
            ++num;
        }
      }
      return num;
    }

    public int getPlayingPlayers(int team, SlotState state, int type, int exception)
    {
      int num = 0;
      lock (this._slots)
      {
        for (int index = 0; index < 16; ++index)
        {
          PointBlank.Core.Models.Room.Slot slot = this._slots[index];
          if (index != exception && slot._playerId > 0L && (type == 0 && slot.state == state || type == 1 && slot.state > state) && (team == 2 || slot._team == team))
            ++num;
        }
      }
      return num;
    }

    public void getPlayingPlayers(bool inBattle, out int RedPlayers, out int BluePlayers)
    {
      RedPlayers = 0;
      BluePlayers = 0;
      lock (this._slots)
      {
        foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
        {
          if (slot._playerId > 0L && (inBattle && slot.state == SlotState.BATTLE && !slot.espectador || !inBattle && slot.state >= SlotState.RENDEZVOUS))
          {
            if (slot._team == 0)
              ++RedPlayers;
            else
              ++BluePlayers;
          }
        }
      }
    }

    public void getPlayingPlayers(
      bool inBattle,
      out int RedPlayers,
      out int BluePlayers,
      out int RedDeaths,
      out int BlueDeaths)
    {
      RedPlayers = 0;
      BluePlayers = 0;
      RedDeaths = 0;
      BlueDeaths = 0;
      lock (this._slots)
      {
        foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
        {
          if (slot._deathState.HasFlag((Enum) DeadEnum.Dead))
          {
            if (slot._team == 0)
              ++RedDeaths;
            else
              ++BlueDeaths;
          }
          if (slot._playerId > 0L && (inBattle && slot.state == SlotState.BATTLE && !slot.espectador || !inBattle && slot.state >= SlotState.RENDEZVOUS))
          {
            if (slot._team == 0)
              ++RedPlayers;
            else
              ++BluePlayers;
          }
        }
      }
    }

    public void CheckToEndWaitingBattle(bool host)
    {
      if (this._state != RoomState.CountDown && this._state != RoomState.Loading && this._state != RoomState.Rendezvous || !host && this._slots[this._leader].state != SlotState.BATTLE_READY)
        return;
      AllUtils.EndBattleNoPoints(this);
    }

    public void SpawnReadyPlayers()
    {
      lock (this._slots)
        this.BaseSpawnReadyPlayers(this.isBotMode());
    }

    public void SpawnReadyPlayers(bool isBotMode)
    {
      lock (this._slots)
        this.BaseSpawnReadyPlayers(isBotMode);
    }

    private void BaseSpawnReadyPlayers(bool isBotMode)
    {
      DateTime now = DateTime.Now;
      foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
      {
        if (slot.state == SlotState.BATTLE_READY && slot.isPlaying == 0 && slot._playerId > 0L)
        {
          slot.isPlaying = 1;
          slot.startTime = now;
          slot.state = SlotState.BATTLE;
          if (this._state == RoomState.Battle && (this.room_type == RoomType.Bomb || this.room_type == RoomType.Annihilation || this.room_type == RoomType.Convoy))
            slot.espectador = true;
        }
      }
      this.updateSlotsInfo();
      List<int> dinossaurs = AllUtils.getDinossaurs(this, false, -1);
      if (this._state == RoomState.PreBattle)
      {
        this.BattleStart = this.room_type == RoomType.Bomb || this.room_type == RoomType.CrossCounter ? now.AddMinutes(5.0) : now;
        this.SetSpecialStage();
      }
      bool flag = false;
      using (PROTOCOL_BATTLE_MISSION_ROUND_PRE_START_ACK roundPreStartAck = new PROTOCOL_BATTLE_MISSION_ROUND_PRE_START_ACK(this, dinossaurs, isBotMode))
      {
        using (PROTOCOL_BATTLE_MISSION_ROUND_START_ACK missionRoundStartAck = new PROTOCOL_BATTLE_MISSION_ROUND_START_ACK(this))
        {
          using (PROTOCOL_BATTLE_RECORD_ACK protocolBattleRecordAck = new PROTOCOL_BATTLE_RECORD_ACK(this))
          {
            byte[] completeBytes1 = roundPreStartAck.GetCompleteBytes("Room.BaseSpawnReadyPlayers-1");
            byte[] completeBytes2 = missionRoundStartAck.GetCompleteBytes("Room.BaseSpawnReadyPlayers-2");
            byte[] completeBytes3 = protocolBattleRecordAck.GetCompleteBytes("Room.BaseSpawnReadyPlayers-3");
            foreach (PointBlank.Core.Models.Room.Slot slot in this._slots)
            {
              PointBlank.Game.Data.Model.Account player;
              if (slot.state == SlotState.BATTLE && slot.isPlaying == 1 && this.getPlayerBySlot(slot, out player))
              {
                slot.isPlaying = 2;
                if (this._state == RoomState.PreBattle)
                {
                  using (PROTOCOL_BATTLE_STARTBATTLE_ACK battleStartbattleAck = new PROTOCOL_BATTLE_STARTBATTLE_ACK(slot, player, dinossaurs, isBotMode, true))
                    this.SendPacketToPlayers((SendPacket) battleStartbattleAck, SlotState.READY, 1);
                  player.SendCompletePacket(completeBytes1);
                  if (this.room_type == RoomType.Boss || this.room_type == RoomType.CrossCounter)
                    flag = true;
                  else
                    player.SendCompletePacket(completeBytes2);
                }
                else if (this._state == RoomState.Battle)
                {
                  using (PROTOCOL_BATTLE_STARTBATTLE_ACK battleStartbattleAck = new PROTOCOL_BATTLE_STARTBATTLE_ACK(slot, player, dinossaurs, isBotMode, false))
                    this.SendPacketToPlayers((SendPacket) battleStartbattleAck, SlotState.READY, 1);
                  if (this.room_type == RoomType.Bomb || this.room_type == RoomType.Annihilation || this.room_type == RoomType.Convoy)
                    GameSync.SendUDPPlayerSync(this, slot, (CouponEffects) 0, 1);
                  else
                    player.SendCompletePacket(completeBytes1);
                  player.SendCompletePacket(completeBytes2);
                  player.SendCompletePacket(completeBytes3);
                }
              }
            }
          }
        }
      }
      if (this._state == RoomState.PreBattle)
      {
        this._state = RoomState.Battle;
        this.updateRoomInfo();
      }
      if (!flag)
        return;
      this.StartDinoRound();
    }

    private void StartDinoRound()
    {
      this.round.Start(5250, (TimerCallback) (callbackState =>
      {
        if (this._state == RoomState.Battle)
        {
          using (PROTOCOL_BATTLE_MISSION_ROUND_START_ACK missionRoundStartAck = new PROTOCOL_BATTLE_MISSION_ROUND_START_ACK(this))
            this.SendPacketToPlayers((SendPacket) missionRoundStartAck, SlotState.BATTLE, 0);
          this.swapRound = false;
        }
        lock (callbackState)
          this.round.Timer = (Timer) null;
      }));
    }
  }
}
