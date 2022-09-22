using PointBlank.Battle.Data.Configs;
using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Xml;
using PointBlank.Battle.Network;
using PointBlank.Battle.Network.Packets;
using System;
using System.Collections.Generic;

namespace PointBlank.Battle.Data.Sync.Client
{
  public static class RespawnSync
  {
    public static void Load(ReceivePacket p)
    {
      uint UniqueRoomId = p.readUD();
      uint Seed = p.readUD();
      long StartTick = p.readQ();
      int num1 = (int) p.readC();
      int num2 = (int) p.readC();
      int Slot = (int) p.readC();
      int num3 = (int) p.readC();
      byte num4 = p.readC();
      int type = 0;
      int charaId = 0;
      int percent = 0;
      bool flag = false;
      if (num1 == 0 || num1 == 2)
      {
        type = (int) p.readC();
        charaId = (int) p.readH();
        percent = (int) p.readC();
        flag = p.readC() == (byte) 1;
        if (28 < p.getBuffer().Length)
          Logger.warning("[RespawnSync]: " + BitConverter.ToString(p.getBuffer()));
      }
      else if (23 < p.getBuffer().Length)
        Logger.warning("[RespawnSync]: " + BitConverter.ToString(p.getBuffer()));
      Room room = RoomsManager.getRoom(UniqueRoomId);
      if (room == null)
        return;
      room.ResyncTick(StartTick, Seed);
      Player player1 = room.getPlayer(Slot, true);
      if (player1 != null && player1.PlayerIdByUser != (int) num4)
        player1.PlayerIdByUser = (int) num4;
      if (player1 == null || player1.PlayerIdByUser != (int) num4)
        return;
      player1.PlayerIdByServer = (int) num4;
      player1.RespawnByServer = num3;
      player1.Integrity = false;
      if (num2 > room.ServerRound)
        room.ServerRound = num2;
      if (num1 == 0 || num1 == 2)
      {
        ++player1.RespawnByLogic;
        player1.Dead = false;
        player1.PlantDuration = BattleConfig.plantDuration;
        player1.DefuseDuration = BattleConfig.defuseDuration;
        if (flag)
        {
          player1.PlantDuration -= AllUtils.Percentage(BattleConfig.plantDuration, 50);
          player1.DefuseDuration -= AllUtils.Percentage(BattleConfig.defuseDuration, 25);
        }
        if (!room.BotMode)
        {
          if (room.SourceToMap == -1)
            room.RoundResetRoomF1(num2);
          else
            room.RoundResetRoomS1(num2);
        }
        if (type == (int) byte.MaxValue)
        {
          player1.Immortal = true;
        }
        else
        {
          player1.Immortal = false;
          int lifeById = CharaXml.getLifeById(charaId, type);
          int num5 = lifeById + AllUtils.Percentage(lifeById, percent);
          player1.MaxLife = num5;
          player1.ResetLife();
        }
      }
      if (room.BotMode || num1 == 2 || !room.ObjectsIsValid())
        return;
      List<ObjectHitInfo> objs = new List<ObjectHitInfo>();
      for (int index = 0; index < room.Objects.Length; ++index)
      {
        ObjectInfo objectInfo = room.Objects[index];
        ObjectModel model = objectInfo.Model;
        if (model != null && (num1 != 2 && model.Destroyable && objectInfo.Life != model.Life || model.NeedSync))
          objs.Add(new ObjectHitInfo(3)
          {
            ObjSyncId = model.NeedSync ? 1 : 0,
            AnimId1 = model.Animation,
            AnimId2 = objectInfo.Animation != null ? objectInfo.Animation.Id : (int) byte.MaxValue,
            DestroyState = objectInfo.DestroyState,
            ObjId = model.Id,
            ObjLife = objectInfo.Life,
            SpecialUse = AllUtils.GetDuration(objectInfo.UseDate)
          });
      }
      for (int index = 0; index < room.Players.Length; ++index)
      {
        Player player2 = room.Players[index];
        if (player2.Slot != Slot && player2.AccountIdIsValid() && !player2.Immortal && player2.Date != new DateTime() && (player2.MaxLife != player2.Life || player2.Dead))
          objs.Add(new ObjectHitInfo(4)
          {
            ObjId = player2.Slot,
            ObjLife = player2.Life
          });
      }
      if (objs.Count > 0)
        BattleManager.Send(PROTOCOL_EVENTS_ACTION.getCode(PROTOCOL_EVENTS_ACTION.getCodeSyncData(objs), room.StartTime, num2, (int) byte.MaxValue), player1.Client);
    }
  }
}
