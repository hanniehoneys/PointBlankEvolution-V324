using PointBlank.Battle.Data;
using PointBlank.Battle.Data.Configs;
using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Models.Event;
using PointBlank.Battle.Data.Sync;
using PointBlank.Battle.Data.Xml;
using PointBlank.Battle.Network.Actions.Damage;
using PointBlank.Battle.Network.Actions.Event;
using PointBlank.Battle.Network.Actions.SubHead;
using PointBlank.Battle.Network.Packets;
using SharpDX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace PointBlank.Battle.Network
{
  public class BattleHandler
  {
    private UdpClient Client;
    private IPEndPoint RemoteEP;

    public BattleHandler(UdpClient Client, byte[] Buff, IPEndPoint RemoteEP, DateTime Date)
    {
      this.Client = Client;
      this.RemoteEP = RemoteEP;
      this.BeginReceive(Buff, Date);
    }

    public void BeginReceive(byte[] Buffer, DateTime Date)
    {
      PacketModel packetModel = new PacketModel();
      packetModel.Data = Buffer;
      packetModel.ReceiveDate = Date;
      ReceivePacket receivePacket = new ReceivePacket(packetModel.Data);
      packetModel.Opcode = (int) receivePacket.readC();
      packetModel.Slot = (int) receivePacket.readC();
      packetModel.Time = receivePacket.readT();
      packetModel.Round = (int) receivePacket.readC();
      packetModel.Length = (int) receivePacket.readUH();
      packetModel.Respawn = (int) receivePacket.readC();
      packetModel.RoundNumber = (int) receivePacket.readC();
      packetModel.AccountId = (int) receivePacket.readC();
      packetModel.Unk = (int) receivePacket.readC();
      if (packetModel.Length > packetModel.Data.Length)
      {
        Logger.LogProblems(this.RemoteEP.ToString(), "Ip/Battle");
        Logger.warning("Packet with invalid size canceled. [Length: " + (object) packetModel.Length + " DataLength: " + (object) packetModel.Data.Length + "]");
      }
      else
      {
        this.getDecryptedData(packetModel);
        if (BattleConfig.isTestMode && packetModel.Unk > 0)
          Logger.warning("Unk: " + (object) packetModel.Unk);
        this.ReadPacket(packetModel);
      }
    }

    public void getDecryptedData(PacketModel packet)
    {
      try
      {
        if (packet.Data.Length < packet.Length)
          throw new Exception("Invalid packet size.");
        byte[] data = new byte[packet.Length - 13];
        Array.Copy((Array) packet.Data, 13, (Array) data, 0, data.Length);
        byte[] numArray1 = AllUtils.Decrypt(data, packet.Length % 6 + 1);
        byte[] numArray2 = new byte[numArray1.Length - 9];
        Array.Copy((Array) numArray1, (Array) numArray2, numArray2.Length);
        packet.withEndData = numArray1;
        packet.noEndData = numArray2;
      }
      catch
      {
      }
    }

    public void ReadPacket(PacketModel Packet)
    {
      byte[] withEndData = Packet.withEndData;
      byte[] noEndData = Packet.noEndData;
      ReceivePacket receivePacket = new ReceivePacket(withEndData);
      int length = noEndData.Length;
      int num1 = 0;
      uint num2 = 0;
      try
      {
        switch (Packet.Opcode)
        {
          case 3:
          case 4:
            receivePacket.Advance(length);
            uint UniqueRoomId1 = receivePacket.readUD();
            int num3 = (int) receivePacket.readC();
            num2 = receivePacket.readUD();
            Room room1 = RoomsManager.getRoom(UniqueRoomId1);
            if (room1 == null)
              break;
            Player player1 = room1.getPlayer(Packet.Slot, this.RemoteEP);
            if (player1 != null && player1.AccountIdIsValid(Packet.AccountId))
            {
              player1.RespawnByUser = Packet.Respawn;
              if (Packet.Opcode == 4)
                room1.BotMode = true;
              if (room1.StartTime == new DateTime())
                break;
              byte[] actions = this.WriteActionBytes(noEndData, room1, AllUtils.GetDuration(player1.Date), Packet);
              bool flag1 = Packet.Opcode == 4 && num3 == (int) byte.MaxValue;
              int slot = 0;
              if (flag1)
                slot = Packet.Slot;
              else if (Packet.Opcode == 3)
                slot = room1.BotMode ? Packet.Slot : (int) byte.MaxValue;
              else
                Logger.warning("DedicationSlotId: " + (object) num3 + " Slot: " + (object) Packet.Slot + "  Opcode: " + (object) Packet.Opcode);
              byte[] code = PROTOCOL_EVENTS_ACTION.getCode(actions, flag1 ? player1.Date : room1.StartTime, Packet.Round, slot);
              bool flag2 = Packet.Opcode == 3 && !room1.BotMode && num3 != (int) byte.MaxValue;
              for (int index = 0; index < 16; ++index)
              {
                bool flag3 = index != Packet.Slot;
                Player player2 = room1.Players[index];
                if (player2.Client != null && player2.AccountIdIsValid() && ((num3 == (int) byte.MaxValue & flag3 ? 1 : (Packet.Opcode != 3 ? 0 : (room1.BotMode ? 1 : 0)) & (flag3 ? 1 : 0)) | (flag2 ? 1 : 0)) != 0)
                  this.Send(code, player2.Client);
              }
            }
            break;
          case 65:
            string Udp = ((int) receivePacket.readH()).ToString() + "." + (object) receivePacket.readH();
            uint UniqueRoomId2 = receivePacket.readUD();
            uint Seed = receivePacket.readUD();
            num1 = (int) receivePacket.readC();
            Room orGetRoom = RoomsManager.CreateOrGetRoom(UniqueRoomId2, Seed);
            if (orGetRoom == null)
              break;
            Player player3 = orGetRoom.AddPlayer(this.RemoteEP, Packet, Udp);
            if (player3 != null)
            {
              if (!player3.Integrity)
                player3.ResetBattleInfos();
              this.Send(PROTOCOL_CONNECT.getCode(), player3.Client);
              if (BattleConfig.isTestMode)
                Logger.warning("Player Connected. [" + (object) player3.Client.Address + ":" + (object) player3.Client.Port + "]");
            }
            break;
          case 67:
            receivePacket.readB(4);
            uint UniqueRoomId3 = receivePacket.readUD();
            num2 = receivePacket.readUD();
            num1 = (int) receivePacket.readC();
            Room room2 = RoomsManager.getRoom(UniqueRoomId3);
            if (room2 == null)
              break;
            if (room2.RemovePlayer(Packet.Slot, this.RemoteEP) && BattleConfig.isTestMode)
              Logger.warning("Player Disconnected. [" + (object) this.RemoteEP.Address + ":" + (object) this.RemoteEP.Port + "]");
            if (room2.getPlayersCount() == 0)
              RoomsManager.RemoveRoom(room2.UniqueRoomId);
            break;
          case 97:
            uint UniqueRoomId4 = receivePacket.readUD();
            num1 = (int) receivePacket.readC();
            num2 = receivePacket.readUD();
            Room room3 = RoomsManager.getRoom(UniqueRoomId4);
            byte[] data1 = Packet.Data;
            if (room3 == null)
              break;
            Player player4 = room3.getPlayer(Packet.Slot, this.RemoteEP);
            if (player4 != null)
            {
              player4.LastPing = Packet.ReceiveDate;
              this.Send(data1, this.RemoteEP);
            }
            break;
          case 131:
          case 132:
            receivePacket.Advance(length);
            uint UniqueRoomId5 = receivePacket.readUD();
            int num4 = (int) receivePacket.readC();
            num2 = receivePacket.readUD();
            Room room4 = RoomsManager.getRoom(UniqueRoomId5);
            if (room4 == null)
              break;
            Player player5 = room4.getPlayer(Packet.Slot, this.RemoteEP);
            if (player5 != null && player5.AccountIdIsValid(Packet.AccountId))
            {
              room4.BotMode = true;
              Player player2 = num4 != (int) byte.MaxValue ? room4.getPlayer(num4, false) : (Player) null;
              byte[] data2 = player2 == null ? PROTOCOL_BOTS_ACTION.getCode(noEndData, player5.Date, Packet.Round, Packet.Slot) : PROTOCOL_BOTS_ACTION.getCode(noEndData, player2.Date, Packet.Round, num4);
              for (int index = 0; index < 16; ++index)
              {
                Player player6 = room4.Players[index];
                if (player6.Client != null && player6.AccountIdIsValid() && index != Packet.Slot)
                  this.Send(data2, player6.Client);
              }
            }
            break;
          default:
            Logger.warning("Invalid Packet: " + (object) Packet.Opcode);
            Logger.warning("Decrypt Data:");
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
        Logger.warning("Decrypt Data:");
      }
    }

    private void RemoveHit(IList List, int Idx)
    {
      List.RemoveAt(Idx);
    }

    public List<ObjectHitInfo> getNewActions(
      ActionModel Action,
      Room Room,
      float Time,
      out byte[] EventsData)
    {
      EventsData = new byte[0];
      if (Room == null)
        return (List<ObjectHitInfo>) null;
      if (Action.Data.Length == 0)
        return new List<ObjectHitInfo>();
      byte[] data = Action.Data;
      List<ObjectHitInfo> objectHitInfoList = new List<ObjectHitInfo>();
      ReceivePacket receivePacket = new ReceivePacket(data);
      using (SendPacket sendPacket = new SendPacket())
      {
        int num1 = 0;
        Player player = Room.getPlayer((int) Action.Slot, true);
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.ActionState))
        {
          ++num1;
          ActionStateInfo info = ActionState.ReadInfo(receivePacket, Action, false);
          if (((ACTION_STATE) info.Action).HasFlag((System.Enum) ACTION_STATE.WEAPONSYNC))
            Room.SyncInfo(objectHitInfoList, 2);
          ActionState.WriteInfo(sendPacket, info);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.Animation))
        {
          num1 += 2;
          Animation.WriteInfo(sendPacket, Action, receivePacket, false);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.PosRotation))
        {
          num1 += 4;
          PosRotationInfo info = PosRotation.ReadInfo(receivePacket, false);
          PosRotation.WriteInfo(sendPacket, info);
          if (player != null)
            player.Position = new Half3(info.RotationX, info.RotationY, info.RotationZ);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.UseObject))
        {
          num1 += 8;
          List<UseObjectInfo> Infos = UseObject.ReadSyncInfo(Action, receivePacket, false);
          for (int index = 0; index < Infos.Count; ++index)
          {
            UseObjectInfo useObjectInfo = Infos[index];
            if (!Room.BotMode && useObjectInfo.ObjectId != ushort.MaxValue && (useObjectInfo.SpaceFlags.HasFlag((System.Enum) CHARA_MOVES.HELI_STOPPED) || useObjectInfo.SpaceFlags.HasFlag((System.Enum) CHARA_MOVES.HELI_IN_MOVE)))
            {
              bool flag = false;
              ObjectInfo objectInfo = Room.getObject((int) useObjectInfo.ObjectId);
              if (objectInfo != null)
              {
                if (useObjectInfo.SpaceFlags.HasFlag((System.Enum) CHARA_MOVES.HELI_STOPPED))
                {
                  AnimModel animation = objectInfo.Animation;
                  if (animation != null && animation.Id == 0)
                    objectInfo.Model.GetAnim(animation.NextAnim, 0.0f, 0.0f, objectInfo);
                }
                else if (useObjectInfo.SpaceFlags.HasFlag((System.Enum) CHARA_MOVES.HELI_IN_MOVE) && objectInfo.UseDate.ToString("yyMMddHHmm") == "0101010000")
                  flag = true;
                if (!flag)
                  objectHitInfoList.Add(new ObjectHitInfo(3)
                  {
                    ObjSyncId = 1,
                    ObjId = objectInfo.Id,
                    ObjLife = objectInfo.Life,
                    AnimId1 = (int) byte.MaxValue,
                    AnimId2 = objectInfo.Animation != null ? objectInfo.Animation.Id : (int) byte.MaxValue,
                    SpecialUse = AllUtils.GetDuration(objectInfo.UseDate)
                  });
              }
            }
            else
              this.RemoveHit((IList) Infos, index--);
          }
          UseObject.WriteInfo(sendPacket, Infos);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.ActionForObjectSync))
        {
          num1 += 16;
          ActionForObjectSync.WriteInfo(sendPacket, Action, receivePacket, false);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.RadioChat))
        {
          num1 += 32;
          RadioChat.WriteInfo(sendPacket, Action, receivePacket, false);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.WeaponSync))
        {
          num1 += 64;
          WeaponSyncInfo info = WeaponSync.ReadInfo(Action, receivePacket, false, false);
          WeaponSync.WriteInfo(sendPacket, info);
          if (player != null)
          {
            player.Extensions = info.Extensions;
            player.WeaponId = info.WeaponId;
            player.WeaponClass = (CLASS_TYPE) AllUtils.getIdStatics(info.WeaponId, 2);
            Room.SyncInfo(objectHitInfoList, 2);
          }
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.HpSync))
        {
          num1 += 256;
          HpSync.writeInfo(sendPacket, receivePacket, false);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.Suicide))
        {
          num1 += 512;
          List<SuicideInfo> hits = Suicide.ReadInfo(receivePacket, false, false);
          int weaponId = 0;
          if (player != null)
          {
            List<DeathServerData> deathServerDataList = new List<DeathServerData>();
            int objId = -1;
            for (int index = 0; index < hits.Count; ++index)
            {
              SuicideInfo suicideInfo = hits[index];
              if (player != null && !player.Dead && player.Life > 0)
              {
                int num2 = (int) (suicideInfo.HitInfo >> 20);
                CHARA_DEATH charaDeath = (CHARA_DEATH) ((int) suicideInfo.HitInfo & 15);
                int num3 = (int) (suicideInfo.HitInfo >> 11) & 511;
                int hitPart = (int) (suicideInfo.HitInfo >> 4) & 63;
                if (((int) (suicideInfo.HitInfo >> 10) & 1) == 1)
                  objId = num3;
                weaponId = suicideInfo.WeaponId;
                player.Life -= num2;
                AssistModel assistModel = new AssistModel();
                assistModel.RoomId = Room.RoomId;
                assistModel.Killer = player.Slot;
                assistModel.Victim = player.Slot;
                assistModel.Damage = num2;
                if (player.Life <= 0)
                {
                  assistModel.IsKiller = true;
                  assistModel.VictimDead = true;
                }
                else
                {
                  assistModel.IsKiller = false;
                  assistModel.VictimDead = false;
                }
                DamageManager.Assists.Add(assistModel);
                if (player.Life <= 0)
                  DamageManager.SetDeath(deathServerDataList, player, player, charaDeath);
                else
                  DamageManager.SetHitEffect(objectHitInfoList, player, charaDeath, hitPart);
                objectHitInfoList.Add(new ObjectHitInfo(2)
                {
                  ObjId = player.Slot,
                  ObjLife = player.Life,
                  DeathType = charaDeath,
                  HitPart = hitPart,
                  WeaponId = weaponId,
                  Position = suicideInfo.PlayerPos
                });
              }
              else
                this.RemoveHit((IList) hits, index--);
            }
            if (deathServerDataList.Count > 0)
              BattleSync.SendDeathSync(Room, player, objId, weaponId, deathServerDataList);
          }
          else
            hits = new List<SuicideInfo>();
          Suicide.WriteInfo(sendPacket, hits);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.MissionData))
        {
          num1 += 1024;
          MissionDataInfo info = MissionData.ReadInfo(Action, receivePacket, false, Time, false);
          if (Room.Map != null && player != null && (!player.Dead && (double) info.PlantTime > 0.0) && !info.BombEnum.HasFlag((System.Enum) BOMB_FLAG.STOP))
          {
            BombPosition bomb = Room.Map.GetBomb(info.BombId);
            if (bomb != null)
            {
              bool flag = info.BombEnum.HasFlag((System.Enum) BOMB_FLAG.DEFUSE);
              Vector3 vector3 = !flag ? (!info.BombEnum.HasFlag((System.Enum) BOMB_FLAG.START) ? (Vector3) new Half3((ushort) 0, (ushort) 0, (ushort) 0) : (Vector3) bomb.Position) : (Vector3) Room.BombPosition;
              double num2 = (double) Vector3.Distance((Vector3) player.Position, vector3);
              if ((bomb.EveryWhere || num2 <= 2.0) && (player.Team == 1 & flag || player.Team == 0 && !flag))
              {
                if ((double) player.C4Time != (double) info.PlantTime)
                {
                  player.C4First = DateTime.Now;
                  player.C4Time = info.PlantTime;
                }
                double totalSeconds = (DateTime.Now - player.C4First).TotalSeconds;
                float num3 = flag ? player.DefuseDuration : player.PlantDuration;
                if (((double) Time >= (double) info.PlantTime + (double) num3 || totalSeconds >= (double) num3) && (!Room.HasC4 && info.BombEnum.HasFlag((System.Enum) BOMB_FLAG.START) || Room.HasC4 & flag))
                {
                  Room.HasC4 = !Room.HasC4;
                  info.Bomb |= 2;
                  MissionData.SendC4UseSync(Room, player, info);
                }
              }
            }
          }
          MissionData.WriteInfo(sendPacket, info);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.DropWeapon))
        {
          num1 += 131072;
          if (Room != null && !Room.BotMode)
          {
            ++Room.DropCounter;
            if (Room.DropCounter > (int) BattleConfig.maxDrop)
              Room.DropCounter = 0;
          }
          DropWeapon.WriteInfo(sendPacket, receivePacket, false, Room != null ? Room.DropCounter : 0);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.GetWeaponForClient))
        {
          num1 += 262144;
          GetWeaponForClient.WriteInfo(sendPacket, Action, receivePacket, false);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.FireData))
        {
          num1 += 524288;
          FireData.WriteInfo(sendPacket, Action, receivePacket, false);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.CharaFireNHitData))
        {
          num1 += 1048576;
          CharaFireNHitData.WriteInfo(sendPacket, receivePacket, false);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.HitData))
        {
          num1 += 2097152;
          List<HitDataInfo> Hits = HitData.ReadInfo(receivePacket, false, false);
          List<DeathServerData> deaths = new List<DeathServerData>();
          int num2 = 0;
          if (player != null)
          {
            for (int index = 0; index < Hits.Count; ++index)
            {
              HitDataInfo hitDataInfo = Hits[index];
              if (hitDataInfo.HitEnum != HIT_TYPE.HelmetProtection && hitDataInfo.HitEnum != HIT_TYPE.HeadshotProtection)
              {
                double num3 = (double) Vector3.Distance((Vector3) hitDataInfo.StartBullet, (Vector3) hitDataInfo.EndBullet);
                if (hitDataInfo.WeaponClass == CLASS_TYPE.Knife && (MeleeExceptionsXml.Contains(AllUtils.getIdStatics(hitDataInfo.WeaponId, 3)) || num3 < 3.0) || hitDataInfo.WeaponClass != CLASS_TYPE.Knife)
                {
                  int damage = (int) AllUtils.getHitDamageNormal(hitDataInfo.HitIndex);
                  int hitWho = AllUtils.getHitWho(hitDataInfo.HitIndex);
                  int hitPart = AllUtils.getHitPart(hitDataInfo.HitIndex);
                  CHARA_DEATH deathType = CHARA_DEATH.DEFAULT;
                  num2 = hitDataInfo.WeaponId;
                  OBJECT_TYPE hitType = AllUtils.getHitType(hitDataInfo.HitIndex);
                  switch (hitType)
                  {
                    case OBJECT_TYPE.User:
                      Player Player;
                      if (Room.getPlayer(hitWho, out Player) && player.RespawnLogicIsValid() && (!player.Dead && !Player.Dead) && !Player.Immortal)
                      {
                        if (hitPart == 24)
                          deathType = CHARA_DEATH.HEADSHOT;
                        if (Room.RoomType == ROOM_STATE_TYPE.DeathMatch && Room.Rule == 32 && deathType != CHARA_DEATH.HEADSHOT)
                          damage = 1;
                        else if (Room.RoomType == ROOM_STATE_TYPE.Boss && deathType == CHARA_DEATH.HEADSHOT)
                        {
                          if (Room.LastRound == 1 && Player.Team == 0 || Room.LastRound == 2 && Player.Team == 1)
                            damage /= 10;
                        }
                        else if (Room.RoomType == ROOM_STATE_TYPE.DeathMatch && Room.Rule == 80)
                          damage = 200;
                        if (BattleConfig.useHitMarker)
                          BattleSync.SendHitMarkerSync(Room, player, (int) deathType, (int) hitDataInfo.HitEnum, damage);
                        AssistModel assistModel = new AssistModel();
                        Player.Life -= damage;
                        assistModel.RoomId = Room.RoomId;
                        assistModel.Killer = player.Slot;
                        assistModel.Victim = Player.Slot;
                        assistModel.Damage = damage;
                        if (Player.Life <= 0)
                        {
                          assistModel.IsKiller = true;
                          assistModel.VictimDead = true;
                        }
                        else
                        {
                          assistModel.IsKiller = false;
                          assistModel.VictimDead = false;
                        }
                        DamageManager.Assists.Add(assistModel);
                        DamageManager.SimpleDeath(deaths, objectHitInfoList, player, Player, damage, num2, hitPart, deathType);
                        break;
                      }
                      this.RemoveHit((IList) Hits, index--);
                      break;
                    case OBJECT_TYPE.UserObject:
                      int num4 = hitWho >> 4;
                      int num5 = hitWho & 15;
                      break;
                    case OBJECT_TYPE.Object:
                      ObjectInfo objectInfo = Room.getObject(hitWho);
                      ObjectModel objM = objectInfo == null ? (ObjectModel) null : objectInfo.Model;
                      if (objM != null && objM.Destroyable)
                      {
                        if (objectInfo.Life > 0)
                        {
                          objectInfo.Life -= damage;
                          if (objectInfo.Life <= 0)
                          {
                            objectInfo.Life = 0;
                            DamageManager.BoomDeath(Room, player, num2, deaths, objectHitInfoList, hitDataInfo.BoomPlayers);
                          }
                          objectInfo.DestroyState = objM.CheckDestroyState(objectInfo.Life);
                          DamageManager.SabotageDestroy(Room, player, objM, objectInfo, damage);
                          objectHitInfoList.Add(new ObjectHitInfo(objM.UpdateId)
                          {
                            ObjId = objectInfo.Id,
                            ObjLife = objectInfo.Life,
                            KillerId = (int) Action.Slot,
                            ObjSyncId = objM.NeedSync ? 1 : 0,
                            SpecialUse = AllUtils.GetDuration(objectInfo.UseDate),
                            AnimId1 = objM.Animation,
                            AnimId2 = objectInfo.Animation != null ? objectInfo.Animation.Id : (int) byte.MaxValue,
                            DestroyState = objectInfo.DestroyState
                          });
                          break;
                        }
                        break;
                      }
                      if (BattleConfig.sendFailMsg && objM == null)
                      {
                        Logger.warning("Fire Obj: " + (object) hitWho + " Map: " + (object) Room.MapId + " Invalid Object.");
                        player.LogPlayerPos(hitDataInfo.EndBullet);
                        break;
                      }
                      break;
                    default:
                      Logger.warning("HitType: (" + (object) hitType + "/" + (object) (int) hitType + ") Slot: " + (object) Action.Slot);
                      Logger.warning("BoomPlayers: " + (object) hitDataInfo.BoomInfo + " " + (object) hitDataInfo.BoomPlayers.Count);
                      break;
                  }
                }
                else
                  this.RemoveHit((IList) Hits, index--);
              }
            }
            if (deaths.Count > 0)
              BattleSync.SendDeathSync(Room, player, (int) byte.MaxValue, num2, deaths);
          }
          else
            Hits = new List<HitDataInfo>();
          HitData.WriteInfo(sendPacket, Hits);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.GrenadeHit))
        {
          num1 += 4194304;
          List<GrenadeHitInfo> hits = GrenadeHit.ReadInfo(receivePacket, false, false);
          List<DeathServerData> deathServerDataList = new List<DeathServerData>();
          int weaponId = 0;
          if (player != null)
          {
            int num2 = -1;
            for (int index = 0; index < hits.Count; ++index)
            {
              GrenadeHitInfo grenadeHitInfo = hits[index];
              int damage = (int) AllUtils.getHitDamageNormal(grenadeHitInfo.HitInfo);
              int hitWho = AllUtils.getHitWho(grenadeHitInfo.HitInfo);
              int hitPart = AllUtils.getHitPart(grenadeHitInfo.HitInfo);
              weaponId = grenadeHitInfo.WeaponId;
              OBJECT_TYPE hitType = AllUtils.getHitType(grenadeHitInfo.HitInfo);
              switch (hitType)
              {
                case OBJECT_TYPE.User:
                  ++num2;
                  Player Player;
                  if (damage > 0 && Room.getPlayer(hitWho, out Player) && (player.RespawnLogicIsValid() && !Player.Dead) && !Player.Immortal)
                  {
                    if (grenadeHitInfo.DeathType == (byte) 10)
                    {
                      Player.Life += damage;
                      Player.CheckLifeValue();
                    }
                    else if (grenadeHitInfo.DeathType == (byte) 2 && CLASS_TYPE.Dino != grenadeHitInfo.WeaponClass && num2 % 2 == 0)
                    {
                      damage = (int) Math.Ceiling((double) damage / 2.7);
                      Player.Life -= damage;
                      AssistModel assistModel = new AssistModel();
                      assistModel.RoomId = Room.RoomId;
                      assistModel.Killer = player.Slot;
                      assistModel.Victim = Player.Slot;
                      assistModel.Damage = damage;
                      if (Player.Life <= 0)
                      {
                        assistModel.IsKiller = true;
                        assistModel.VictimDead = true;
                      }
                      else
                      {
                        assistModel.IsKiller = false;
                        assistModel.VictimDead = false;
                      }
                      DamageManager.Assists.Add(assistModel);
                      if (Player.Life <= 0)
                        DamageManager.SetDeath(deathServerDataList, Player, player, (CHARA_DEATH) grenadeHitInfo.DeathType);
                      else
                        DamageManager.SetHitEffect(objectHitInfoList, Player, player, (CHARA_DEATH) grenadeHitInfo.DeathType, hitPart);
                    }
                    else
                    {
                      Player.Life -= damage;
                      AssistModel assistModel = new AssistModel();
                      assistModel.RoomId = Room.RoomId;
                      assistModel.Killer = player.Slot;
                      assistModel.Victim = Player.Slot;
                      assistModel.Damage = damage;
                      if (Player.Life <= 0)
                      {
                        assistModel.IsKiller = true;
                        assistModel.VictimDead = true;
                      }
                      else
                      {
                        assistModel.IsKiller = false;
                        assistModel.VictimDead = false;
                      }
                      DamageManager.Assists.Add(assistModel);
                      if (Player.Life <= 0)
                        DamageManager.SetDeath(deathServerDataList, Player, player, (CHARA_DEATH) grenadeHitInfo.DeathType);
                      else
                        DamageManager.SetHitEffect(objectHitInfoList, Player, player, (CHARA_DEATH) grenadeHitInfo.DeathType, hitPart);
                    }
                    if (damage > 0)
                    {
                      if (BattleConfig.useHitMarker)
                        BattleSync.SendHitMarkerSync(Room, player, (int) grenadeHitInfo.DeathType, (int) grenadeHitInfo.HitEnum, damage);
                      objectHitInfoList.Add(new ObjectHitInfo(2)
                      {
                        ObjId = Player.Slot,
                        ObjLife = Player.Life,
                        DeathType = (CHARA_DEATH) grenadeHitInfo.DeathType,
                        WeaponId = weaponId,
                        HitPart = hitPart
                      });
                      break;
                    }
                    break;
                  }
                  this.RemoveHit((IList) hits, index--);
                  break;
                case OBJECT_TYPE.UserObject:
                  int num3 = hitWho >> 4;
                  int num4 = hitWho & 15;
                  break;
                case OBJECT_TYPE.Object:
                  ObjectInfo objectInfo = Room.getObject(hitWho);
                  ObjectModel objM = objectInfo == null ? (ObjectModel) null : objectInfo.Model;
                  if (objM != null && objM.Destroyable && objectInfo.Life > 0)
                  {
                    objectInfo.Life -= damage;
                    if (objectInfo.Life <= 0)
                    {
                      objectInfo.Life = 0;
                      DamageManager.BoomDeath(Room, player, weaponId, deathServerDataList, objectHitInfoList, grenadeHitInfo.BoomPlayers);
                    }
                    objectInfo.DestroyState = objM.CheckDestroyState(objectInfo.Life);
                    DamageManager.SabotageDestroy(Room, player, objM, objectInfo, damage);
                    if (damage > 0)
                    {
                      objectHitInfoList.Add(new ObjectHitInfo(objM.UpdateId)
                      {
                        ObjId = objectInfo.Id,
                        ObjLife = objectInfo.Life,
                        KillerId = (int) Action.Slot,
                        ObjSyncId = objM.NeedSync ? 1 : 0,
                        AnimId1 = objM.Animation,
                        AnimId2 = objectInfo.Animation != null ? objectInfo.Animation.Id : (int) byte.MaxValue,
                        DestroyState = objectInfo.DestroyState,
                        SpecialUse = AllUtils.GetDuration(objectInfo.UseDate)
                      });
                      break;
                    }
                    break;
                  }
                  if (BattleConfig.sendFailMsg && objM == null)
                  {
                    Logger.warning("Boom Obj: " + (object) hitWho + " Map: " + (object) Room.MapId + " Invalid Object.");
                    player.LogPlayerPos(grenadeHitInfo.HitPos);
                    break;
                  }
                  break;
                default:
                  Logger.warning("Grenade Boom, HitType: (" + (object) hitType + "/" + (object) (int) hitType + ")");
                  break;
              }
            }
            if (deathServerDataList.Count > 0)
              BattleSync.SendDeathSync(Room, player, (int) byte.MaxValue, weaponId, deathServerDataList);
          }
          else
            hits = new List<GrenadeHitInfo>();
          GrenadeHit.WriteInfo(sendPacket, hits);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.GetWeaponForHost))
        {
          num1 += 16777216;
          GetWeaponForHost.WriteInfo(sendPacket, Action, receivePacket, false);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.FireDataOnObject))
        {
          num1 += 33554432;
          FireDataOnObject.WriteInfo(sendPacket, receivePacket, false);
        }
        if (Action.Flag.HasFlag((System.Enum) UDP_GAME_EVENTS.FireNHitDataOnObject))
        {
          num1 += 67108864;
          FireNHitDataObjectInfo info = FireNHitDataOnObject.ReadInfo(Action, receivePacket, false);
          FireNHitDataOnObject.WriteInfo(sendPacket, info);
          if (player != null && !player.Dead)
            FireNHitDataOnObject.SendPassSync(Room, player, info);
        }
        EventsData = sendPacket.mstream.ToArray();
        if ((long) num1 != (long) Action.Flag)
          Logger.warning("[" + (object) (uint) Action.Flag + " | " + (object) ((long) Action.Flag - (long) num1));
        return objectHitInfoList;
      }
    }

    public void CheckDataFlags(ActionModel Action, PacketModel Packet)
    {
      UDP_GAME_EVENTS flag = Action.Flag;
      if (!flag.HasFlag((System.Enum) UDP_GAME_EVENTS.WeaponSync) || Packet.Opcode == 4 || (flag & (UDP_GAME_EVENTS.DropWeapon | UDP_GAME_EVENTS.GetWeaponForClient)) <= (UDP_GAME_EVENTS) 0)
        return;
      Action.Flag -= UDP_GAME_EVENTS.WeaponSync;
    }

    public byte[] WriteActionBytes(byte[] Data, Room Room, float time, PacketModel Packet)
    {
      ReceivePacket p = new ReceivePacket(Data);
      List<ObjectHitInfo> objs = new List<ObjectHitInfo>();
      using (SendPacket s = new SendPacket())
      {
        for (int index = 0; index < 16; ++index)
        {
          ActionModel Action = new ActionModel();
          try
          {
            bool exception;
            Action.SubHead = (UDP_SUB_HEAD) p.readC(out exception);
            if (!exception)
            {
              Action.Slot = p.readUH();
              Action.Length = p.readUH();
              if (Action.Length != ushort.MaxValue)
              {
                s.writeC((byte) Action.SubHead);
                s.writeH(Action.Slot);
                s.writeH(Action.Length);
                if (Action.SubHead == UDP_SUB_HEAD.GRENADE)
                  GrenadeSync.WriteInfo(s, p, false);
                else if (Action.SubHead == UDP_SUB_HEAD.DROPEDWEAPON)
                  DropedWeapon.WriteInfo(s, p, false);
                else if (Action.SubHead == UDP_SUB_HEAD.OBJECT_STATIC)
                  ObjectStatic.WriteInfo(s, p, false);
                else if (Action.SubHead == UDP_SUB_HEAD.OBJECT_ANIM)
                  ObjectAnim.WriteInfo(s, p, false);
                else if (Action.SubHead == UDP_SUB_HEAD.STAGEINFO_OBJ_STATIC)
                  StageInfoObjStatic.WriteInfo(s, p, false);
                else if (Action.SubHead == UDP_SUB_HEAD.STAGEINFO_OBJ_ANIM)
                  StageObjAnim.WriteInfo(s, p, false);
                else if (Action.SubHead == UDP_SUB_HEAD.CONTROLED_OBJECT)
                  ControledObj.WriteInfo(s, p, false);
                else if (Action.SubHead == UDP_SUB_HEAD.USER || Action.SubHead == UDP_SUB_HEAD.STAGEINFO_CHARA)
                {
                  Action.Flag = (UDP_GAME_EVENTS) p.readUD();
                  Action.Data = p.readB((int) Action.Length - 9);
                  this.CheckDataFlags(Action, Packet);
                  byte[] EventsData;
                  objs.AddRange((IEnumerable<ObjectHitInfo>) this.getNewActions(Action, Room, time, out EventsData));
                  s.GoBack(2);
                  s.writeH((ushort) (EventsData.Length + 9));
                  s.writeD((uint) Action.Flag);
                  s.writeB(EventsData);
                  if (Action.Data.Length == 0 && (uint) Action.Length - 9U > 0U)
                    break;
                }
                else
                  Logger.warning("[SubHead: '" + (object) Action.SubHead + "' or '" + (object) (int) Action.SubHead + "']");
              }
              else
                break;
            }
            else
              break;
          }
          catch (Exception ex)
          {
            Logger.warning("WriteActionBytes \r\n" + ex.ToString());
            Logger.warning("WriteActionBytes Data: ");
            objs = new List<ObjectHitInfo>();
            break;
          }
        }
        if (objs.Count > 0)
          s.writeB(PROTOCOL_EVENTS_ACTION.getCodeSyncData(objs));
        return s.mstream.ToArray();
      }
    }

    private void Send(byte[] data, IPEndPoint ip)
    {
      this.Client.Send(data, data.Length, ip);
    }
  }
}
