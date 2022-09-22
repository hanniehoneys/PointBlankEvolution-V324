using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Sync;
using SharpDX;
using System;
using System.Collections.Generic;

namespace PointBlank.Battle.Network.Actions.Damage
{
  public class DamageManager
  {
    public static List<AssistModel> Assists = new List<AssistModel>();

    public static void SabotageDestroy(
      Room room,
      Player pl,
      ObjectModel objM,
      ObjectInfo obj,
      int damage)
    {
      if (objM.UltraSync <= 0 || room.RoomType != ROOM_STATE_TYPE.Destroy && room.RoomType != ROOM_STATE_TYPE.Defense)
        return;
      if (objM.UltraSync == 1 || objM.UltraSync == 3)
        room.Bar1 = obj.Life;
      else if (objM.UltraSync == 2 || objM.UltraSync == 4)
        room.Bar2 = obj.Life;
      BattleSync.SendSabotageSync(room, pl, damage, objM.UltraSync == 4 ? 2 : 1);
    }

    public static void SetDeath(
      List<DeathServerData> Deaths,
      Player Player,
      Player Killer,
      CHARA_DEATH DeathType)
    {
      lock (DamageManager.Assists)
      {
        AssistModel assistModel = DamageManager.Assists.Find((Predicate<AssistModel>) (x => x.Victim == Player.Slot));
        Player.Life = 0;
        Player.Dead = true;
        Deaths.Add(new DeathServerData()
        {
          Player = Player,
          DeathType = DeathType,
          Assist = assistModel == null ? Killer.Slot : (assistModel.IsAssist ? assistModel.Killer : Killer.Slot)
        });
        DamageManager.Assists.Remove(assistModel);
      }
    }

    public static void SetHitEffect(
      List<ObjectHitInfo> objs,
      Player player,
      Player killer,
      CHARA_DEATH deathType,
      int hitPart)
    {
      objs.Add(new ObjectHitInfo(2)
      {
        ObjId = player.Slot,
        KillerId = killer.Slot,
        DeathType = deathType,
        ObjLife = player.Life,
        HitPart = hitPart
      });
    }

    public static void SetHitEffect(
      List<ObjectHitInfo> objs,
      Player player,
      CHARA_DEATH deathType,
      int hitPart)
    {
      objs.Add(new ObjectHitInfo(2)
      {
        ObjId = player.Slot,
        KillerId = player.Slot,
        DeathType = deathType,
        ObjLife = player.Life,
        HitPart = hitPart
      });
    }

    public static void BoomDeath(
      Room room,
      Player pl,
      int weaponId,
      List<DeathServerData> deaths,
      List<ObjectHitInfo> objs,
      List<int> BoomPlayers)
    {
      if (BoomPlayers == null || BoomPlayers.Count == 0)
        return;
      for (int index = 0; index < BoomPlayers.Count; ++index)
      {
        int boomPlayer = BoomPlayers[index];
        Player Player;
        if (room.getPlayer(boomPlayer, out Player) && !Player.Dead)
        {
          DamageManager.SetDeath(deaths, Player, pl, CHARA_DEATH.OBJECT_EXPLOSION);
          objs.Add(new ObjectHitInfo(2)
          {
            HitPart = 1,
            DeathType = CHARA_DEATH.OBJECT_EXPLOSION,
            ObjId = boomPlayer,
            KillerId = pl.Slot,
            WeaponId = weaponId
          });
        }
      }
    }

    public static void SimpleDeath(
      List<DeathServerData> deaths,
      List<ObjectHitInfo> objs,
      Player killer,
      Player victim,
      int damage,
      int weapon,
      int hitPart,
      CHARA_DEATH deathType)
    {
      lock (DamageManager.Assists)
      {
        foreach (AssistModel assistModel in DamageManager.Assists.FindAll((Predicate<AssistModel>) (x => x.Victim == victim.Slot)))
        {
          if (!assistModel.IsKiller)
            assistModel.IsAssist = true;
        }
      }
      if (victim.Life <= 0)
        DamageManager.SetDeath(deaths, victim, killer, deathType);
      else
        DamageManager.SetHitEffect(objs, victim, killer, deathType, hitPart);
      objs.Add(new ObjectHitInfo(2)
      {
        ObjId = victim.Slot,
        ObjLife = victim.Life,
        HitPart = hitPart,
        KillerId = killer.Slot,
        Position = (Half3) ((Vector3) victim.Position - (Vector3) killer.Position),
        DeathType = deathType,
        WeaponId = weapon
      });
    }
  }
}
