using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Data.Xml;
using System;

namespace PointBlank.Game.Data.Sync.Client
{
  public static class RoomDeath
  {
    public static void Load(ReceiveGPacket p)
    {
      int id1 = (int) p.readH();
      int id2 = (int) p.readH();
      byte num1 = p.readC();
      byte num2 = p.readC();
      int num3 = p.readD();
      float num4 = p.readT();
      float num5 = p.readT();
      float num6 = p.readT();
      byte num7 = p.readC();
      int num8 = (int) num7 * 15;
      if (p.getBuffer().Length > 25 + num8)
        Logger.warning("Invalid Death: " + BitConverter.ToString(p.getBuffer()));
      Channel channel = ChannelsXml.getChannel(id2);
      if (channel == null)
        return;
      PointBlank.Game.Data.Model.Room room = channel.getRoom(id1);
      if (room == null || room.round.Timer != null || room._state != RoomState.Battle)
        return;
      Slot slot1 = room.getSlot((int) num1);
      if (slot1 == null || slot1.state != SlotState.BATTLE)
        return;
      FragInfos kills = new FragInfos()
      {
        killerIdx = num1,
        killingType = CharaKillType.DEFAULT,
        weapon = num3,
        x = num4,
        y = num5,
        z = num6,
        flag = num2
      };
      bool isSuicide = false;
      for (int index = 0; index < (int) num7; ++index)
      {
        byte num9 = p.readC();
        byte hitspotInfo = p.readC();
        float num10 = p.readT();
        float num11 = p.readT();
        float num12 = p.readT();
        int num13 = (int) p.readC();
        int slotIdx = (int) hitspotInfo & 15;
        Slot slot2 = room.getSlot(slotIdx);
        if (slot2 != null && slot2.state == SlotState.BATTLE)
        {
          Frag frag = new Frag(hitspotInfo)
          {
            flag = 0,
            AssistSlot = num13,
            victimWeaponClass = num9,
            x = num10,
            y = num11,
            z = num12
          };
          if ((int) kills.killerIdx == slotIdx)
            isSuicide = true;
          kills.frags.Add(frag);
        }
      }
      kills.killsCount = (byte) kills.frags.Count;
      GameSync.genDeath(room, slot1, kills, isSuicide);
    }

    public static void RegistryFragInfos(
      PointBlank.Game.Data.Model.Room room,
      Slot killer,
      out int score,
      bool isBotMode,
      bool isSuicide,
      FragInfos kills)
    {
      score = 0;
      ItemClass idStatics1 = (ItemClass) ComDiv.getIdStatics(kills.weapon, 1);
      ClassType idStatics2 = (ClassType) ComDiv.getIdStatics(kills.weapon, 2);
      for (int index = 0; index < kills.frags.Count; ++index)
      {
        Frag frag = kills.frags[index];
        CharaDeath charaDeath = (CharaDeath) ((int) frag.hitspotInfo >> 4);
        if ((int) kills.killsCount - (isSuicide ? 1 : 0) > 1)
        {
          frag.killFlag |= charaDeath == CharaDeath.BOOM || charaDeath == CharaDeath.OBJECT_EXPLOSION || (charaDeath == CharaDeath.POISON || charaDeath == CharaDeath.HOWL) || (charaDeath == CharaDeath.TRAMPLED || idStatics2 == ClassType.Shotgun) ? KillingMessage.MassKill : KillingMessage.PiercingShot;
        }
        else
        {
          int num1 = 0;
          switch (charaDeath)
          {
            case CharaDeath.DEFAULT:
              if (idStatics1 == ItemClass.KNIFE)
              {
                num1 = 6;
                break;
              }
              break;
            case CharaDeath.HEADSHOT:
              num1 = 4;
              break;
          }
          if (num1 > 0)
          {
            int num2 = killer.lastKillState >> 12;
            switch (num1)
            {
              case 4:
                if (num2 != 4)
                  killer.repeatLastState = false;
                killer.lastKillState = num1 << 12 | killer.killsOnLife + 1;
                if (killer.repeatLastState)
                {
                  frag.killFlag |= (killer.lastKillState & 16383) <= 1 ? KillingMessage.Headshot : KillingMessage.ChainHeadshot;
                  break;
                }
                frag.killFlag |= KillingMessage.Headshot;
                killer.repeatLastState = true;
                break;
              case 6:
                if (num2 != 6)
                  killer.repeatLastState = false;
                killer.lastKillState = num1 << 12 | killer.killsOnLife + 1;
                if (killer.repeatLastState && (killer.lastKillState & 16383) > 1)
                {
                  frag.killFlag |= KillingMessage.ChainSlugger;
                  break;
                }
                killer.repeatLastState = true;
                break;
            }
          }
          else
          {
            killer.lastKillState = 0;
            killer.repeatLastState = false;
          }
        }
        int victimSlot = frag.VictimSlot;
        int assistSlot = frag.AssistSlot;
        Slot slot1 = room._slots[victimSlot];
        Slot slot2 = room._slots[assistSlot];
        if (slot1.killsOnLife > 3)
          frag.killFlag |= KillingMessage.ChainStopper;
        if ((kills.weapon != 19016 && kills.weapon != 19022 || (int) kills.killerIdx != victimSlot) && !slot1.specGM)
          ++slot1.allDeaths;
        if ((int) kills.killerIdx != assistSlot)
          ++slot2.allAssists;
        if (room.room_type == RoomType.FreeForAll)
        {
          ++killer.allKills;
          if (killer._deathState == DeadEnum.Alive)
            ++killer.killsOnLife;
        }
        else if (killer._team != slot1._team)
        {
          score += AllUtils.getKillScore(frag.killFlag);
          ++killer.allKills;
          if (killer._deathState == DeadEnum.Alive)
            ++killer.killsOnLife;
          if (slot1._team == 0)
          {
            ++room._redDeaths;
            ++room._blueKills;
          }
          else
          {
            ++room._blueDeaths;
            ++room._redKills;
          }
          if (room.room_type == RoomType.Boss)
          {
            if (killer._team == 0)
              room.red_dino += 4;
            else
              room.blue_dino += 4;
          }
        }
        slot1.lastKillState = 0;
        slot1.killsOnLife = 0;
        slot1.repeatLastState = false;
        slot1.passSequence = 0;
        slot1._deathState = DeadEnum.Dead;
        if (!isBotMode)
          AllUtils.CompleteMission(room, slot1, MissionType.DEATH, 0);
        if (charaDeath == CharaDeath.HEADSHOT)
          ++killer.headshots;
      }
    }

    public static void EndBattleByDeath(PointBlank.Game.Data.Model.Room room, Slot killer, bool isBotMode, bool isSuicide)
    {
      if (room.room_type == RoomType.DeathMatch && !isBotMode)
        AllUtils.BattleEndKills(room, isBotMode);
      else if (room.room_type == RoomType.FreeForAll)
      {
        AllUtils.BattleEndKillsFreeForAll(room);
      }
      else
      {
        if (killer.specGM || room.room_type != RoomType.Bomb && room.room_type != RoomType.Annihilation && room.room_type != RoomType.Convoy)
          return;
        int winner1 = 0;
        int RedPlayers;
        int BluePlayers;
        int RedDeaths;
        int BlueDeaths;
        room.getPlayingPlayers(true, out RedPlayers, out BluePlayers, out RedDeaths, out BlueDeaths);
        if (((RedDeaths != RedPlayers ? 0 : (killer._team == 0 ? 1 : 0)) & (isSuicide ? 1 : 0)) != 0 && !room.C4_actived)
        {
          int winner2 = 1;
          ++room.blue_rounds;
          AllUtils.BattleEndRound(room, winner2, true);
        }
        else if (BlueDeaths == BluePlayers && killer._team == 1)
        {
          ++room.red_rounds;
          AllUtils.BattleEndRound(room, winner1, true);
        }
        else if (RedDeaths == RedPlayers && killer._team == 1)
        {
          if (!room.C4_actived)
          {
            winner1 = 1;
            ++room.blue_rounds;
          }
          else if (isSuicide)
            ++room.red_rounds;
          AllUtils.BattleEndRound(room, winner1, false);
        }
        else
        {
          if (BlueDeaths != BluePlayers || killer._team != 0)
            return;
          if (!isSuicide || !room.C4_actived)
          {
            ++room.red_rounds;
          }
          else
          {
            winner1 = 1;
            ++room.blue_rounds;
          }
          AllUtils.BattleEndRound(room, winner1, true);
        }
      }
    }
  }
}
