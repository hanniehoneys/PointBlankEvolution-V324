using PointBlank.Battle.Data.Configs;
using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Xml;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Net;

namespace PointBlank.Battle.Data.Models
{
  public class Room
  {
    public Player[] Players = new Player[16];
    public ObjectInfo[] Objects = new ObjectInfo[200];
    public int SourceToMap = -1;
    public int Bar1 = 6000;
    public int Bar2 = 6000;
    public int Default1 = 6000;
    public int Default2 = 6000;
    private object Lock = new object();
    private object Lock2 = new object();
    public uint UniqueRoomId;
    public uint Seed;
    public int ObjsSyncRound;
    public int ServerRound;
    public int ServerId;
    public int Rule;
    public int RoomId;
    public int ChannelId;
    public int LastRound;
    public int DropCounter;
    public MAP_STATE_ID MapId;
    public ROOM_STATE_TYPE RoomType;
    public GameServerModel GameServer;
    public MapModel Map;
    public bool BotMode;
    public bool HasC4;
    public long LastStartTick;
    public Half3 BombPosition;
    public DateTime StartTime;

    public Room(int ServerId)
    {
      this.GameServer = ServersXml.getServer(ServerId);
      if (this.GameServer == null)
        return;
      this.ServerId = ServerId;
      for (int Slot = 0; Slot < 16; ++Slot)
        this.Players[Slot] = new Player(Slot);
      for (int Id = 0; Id < 200; ++Id)
        this.Objects[Id] = new ObjectInfo(Id);
    }

    public void SyncInfo(List<ObjectHitInfo> Objs, int Type)
    {
      lock (this.Lock2)
      {
        if (this.BotMode || !this.ObjectsIsValid())
          return;
        DateTime now = DateTime.Now;
        if ((Type & 1) == 1)
        {
          for (int index = 0; index < this.Objects.Length; ++index)
          {
            ObjectInfo objectInfo = this.Objects[index];
            ObjectModel model = objectInfo.Model;
            if (model != null && (model.Destroyable && objectInfo.Life != model.Life || model.NeedSync))
            {
              float duration = AllUtils.GetDuration(objectInfo.UseDate);
              AnimModel animation = objectInfo.Animation;
              if (animation != null && (double) animation.Duration > 0.0 && (double) duration >= (double) animation.Duration)
                model.GetAnim(animation.NextAnim, duration, animation.Duration, objectInfo);
              Objs.Add(new ObjectHitInfo(model.UpdateId)
              {
                ObjSyncId = model.NeedSync ? 1 : 0,
                AnimId1 = model.Animation,
                AnimId2 = objectInfo.Animation != null ? objectInfo.Animation.Id : (int) byte.MaxValue,
                DestroyState = objectInfo.DestroyState,
                ObjId = model.Animation,
                ObjLife = objectInfo.Life,
                SpecialUse = duration
              });
            }
          }
        }
        if ((Type & 2) != 2)
          return;
        for (int index = 0; index < this.Players.Length; ++index)
        {
          Player player = this.Players[index];
          if (!player.Immortal && (player.MaxLife != player.Life || player.Dead))
            Objs.Add(new ObjectHitInfo(4)
            {
              ObjId = player.Slot,
              ObjLife = player.Life
            });
        }
      }
    }

    public bool ObjectsIsValid()
    {
      return this.ServerRound == this.ObjsSyncRound;
    }

    public void ResyncTick(long StartTick, uint Seed)
    {
      if (StartTick <= this.LastStartTick)
        return;
      this.StartTime = new DateTime(StartTick);
      if (this.LastStartTick > 0L)
        this.ResetRoomInfo(Seed);
      this.LastStartTick = StartTick;
      if (BattleConfig.isTestMode)
        Logger.warning("[New tick is defined]");
    }

    public void ResetRoomInfo(uint Seed)
    {
      for (int Id = 0; Id < 200; ++Id)
        this.Objects[Id] = new ObjectInfo(Id);
      this.MapId = (MAP_STATE_ID) AllUtils.GetSeedInfo(Seed, 2);
      this.RoomType = (ROOM_STATE_TYPE) AllUtils.GetSeedInfo(Seed, 0);
      this.Rule = AllUtils.GetSeedInfo(Seed, 1);
      this.SourceToMap = -1;
      this.Map = (MapModel) null;
      this.LastRound = 0;
      this.DropCounter = 0;
      this.BotMode = false;
      this.HasC4 = false;
      this.ServerRound = 0;
      this.ObjsSyncRound = 0;
      this.BombPosition = new Half3();
      if (!BattleConfig.isTestMode)
        return;
      Logger.warning("A room has been reseted by server.");
    }

    public bool RoundResetRoomF1(int Round)
    {
      lock (this.Lock)
      {
        if (this.LastRound != Round)
        {
          if (BattleConfig.isTestMode)
            Logger.warning("Reseting room. [Last: " + (object) this.LastRound + "; New: " + (object) Round + "]");
          DateTime now = DateTime.Now;
          this.LastRound = Round;
          this.HasC4 = false;
          this.BombPosition = new Half3();
          this.DropCounter = 0;
          this.ObjsSyncRound = 0;
          this.SourceToMap = (int) this.MapId;
          if (!this.BotMode)
          {
            for (int index = 0; index < 16; ++index)
            {
              Player player = this.Players[index];
              player.Life = player.MaxLife;
            }
            this.Map = MapXml.getMapId((int) this.MapId);
            List<ObjectModel> objectModelList = this.Map != null ? this.Map.Objects : (List<ObjectModel>) null;
            if (objectModelList != null)
            {
              for (int index = 0; index < objectModelList.Count; ++index)
              {
                ObjectModel objectModel = objectModelList[index];
                ObjectInfo objectInfo = this.Objects[objectModel.Id];
                objectInfo.Life = objectModel.Life;
                if (!objectModel.NoInstaSync)
                {
                  objectModel.GetRandomAnimation(this, objectInfo);
                }
                else
                {
                  objectInfo.Animation = new AnimModel()
                  {
                    NextAnim = 1
                  };
                  objectInfo.UseDate = now;
                }
                objectInfo.Model = objectModel;
                objectInfo.DestroyState = 0;
                MapXml.SetObjectives(objectModel, this);
              }
            }
            this.ObjsSyncRound = Round;
          }
          return true;
        }
      }
      return false;
    }

    public bool RoundResetRoomS1(int Round)
    {
      lock (this.Lock)
      {
        if (this.LastRound != Round)
        {
          if (BattleConfig.isTestMode)
            Logger.warning("Reseting room. [Last: " + (object) this.LastRound + "; New: " + (object) Round + "]");
          this.LastRound = Round;
          this.HasC4 = false;
          this.DropCounter = 0;
          this.BombPosition = new Half3();
          if (!this.BotMode)
          {
            for (int index = 0; index < 16; ++index)
            {
              Player player = this.Players[index];
              player.Life = player.MaxLife;
            }
            DateTime now = DateTime.Now;
            for (int index = 0; index < this.Objects.Length; ++index)
            {
              ObjectInfo objectInfo = this.Objects[index];
              ObjectModel model = objectInfo.Model;
              if (model != null)
              {
                objectInfo.Life = model.Life;
                if (!model.NoInstaSync)
                {
                  model.GetRandomAnimation(this, objectInfo);
                }
                else
                {
                  objectInfo.Animation = new AnimModel()
                  {
                    NextAnim = 1
                  };
                  objectInfo.UseDate = now;
                }
                objectInfo.DestroyState = 0;
              }
            }
            this.ObjsSyncRound = Round;
            if (this.RoomType == ROOM_STATE_TYPE.Destroy || this.RoomType == ROOM_STATE_TYPE.Defense)
            {
              this.Bar1 = this.Default1;
              this.Bar2 = this.Default2;
            }
          }
          return true;
        }
      }
      return false;
    }

    public Player AddPlayer(IPEndPoint Client, PacketModel Packet, string Udp)
    {
      if (BattleConfig.udpVersion != Udp)
        return (Player) null;
      try
      {
        Player player = this.Players[Packet.Slot];
        if (!player.CompareIp(Client))
        {
          player.Client = Client;
          player.Date = Packet.ReceiveDate;
          player.PlayerIdByUser = Packet.AccountId;
          return player;
        }
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
      return (Player) null;
    }

    public bool getPlayer(int Slot, out Player Player)
    {
      try
      {
        Player = this.Players[Slot];
      }
      catch
      {
        Player = (Player) null;
      }
      return Player != null;
    }

    public Player getPlayer(int Slot, bool Active)
    {
      Player player;
      try
      {
        player = this.Players[Slot];
      }
      catch
      {
        player = (Player) null;
      }
      return player == null || Active && player.Client == null ? (Player) null : player;
    }

    public Player getPlayer(int Slot, IPEndPoint Client)
    {
      Player player;
      try
      {
        player = this.Players[Slot];
      }
      catch
      {
        player = (Player) null;
      }
      return player == null || !player.CompareIp(Client) ? (Player) null : player;
    }

    public ObjectInfo getObject(int Id)
    {
      ObjectInfo objectInfo;
      try
      {
        objectInfo = this.Objects[Id];
      }
      catch
      {
        objectInfo = (ObjectInfo) null;
      }
      return objectInfo;
    }

    public bool RemovePlayer(int Slot, IPEndPoint Ip)
    {
      try
      {
        Player player = this.Players[Slot];
        if (!player.CompareIp(Ip))
          return false;
        player.ResetAllInfos();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public int getPlayersCount()
    {
      int num = 0;
      for (int index = 0; index < 16; ++index)
      {
        if (this.Players[index].Client != null)
          ++num;
      }
      return num;
    }
  }
}
