using PointBlank.Core;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Sync.Client;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PointBlank.Game.Data.Sync
{
  public static class GameSync
  {
    private static DateTime LastSyncCount;
    public static UdpClient udp;

    public static void Start()
    {
      try
      {
        udp = new UdpClient(GameConfig.syncPort);
        uint IOC_IN = 0x80000000;
        uint IOC_VENDOR = 0x18000000;
        uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
        udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
        new Thread(Read).Start();
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    public static void Read()
    {
      try
      {
        GameSync.udp.BeginReceive(new AsyncCallback(GameSync.AcceptCallback), (object) null);
      }
      catch
      {
      }
    }

    private static void AcceptCallback(IAsyncResult res)
    {
      if (GameManager.ServerIsClosed)
        return;
      IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 8000);
      byte[] buffer = GameSync.udp.EndReceive(res, ref remoteEP);
      Thread.Sleep(5);
      new Thread(new ThreadStart(GameSync.Read)).Start();
      if (buffer.Length < 2)
        return;
      GameSync.LoadPacket(buffer);
    }

    private static void LoadPacket(byte[] buffer)
    {
      ReceiveGPacket p = new ReceiveGPacket(buffer);
      short num1 = p.readH();
      try
      {
        switch (num1)
        {
          case 1:
            RoomPassPortal.Load(p);
            break;
          case 2:
            RoomC4.Load(p);
            break;
          case 3:
            RoomDeath.Load(p);
            break;
          case 4:
            RoomHitMarker.Load(p);
            break;
          case 5:
            RoomSabotageSync.Load(p);
            break;
          case 10:
            PointBlank.Game.Data.Model.Account account1 = AccountManager.getAccount(p.readQ(), true);
            if (account1 == null)
              break;
            account1.SendPacket((SendPacket) new PROTOCOL_AUTH_ACCOUNT_KICK_ACK(1));
            account1.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ERROR_ACK(2147487744U));
            account1.Close(1000, false);
            break;
          case 11:
            int num2 = (int) p.readC();
            int num3 = (int) p.readC();
            PointBlank.Game.Data.Model.Account account2 = AccountManager.getAccount(p.readQ(), 0);
            if (account2 == null)
              break;
            PointBlank.Game.Data.Model.Account account3 = AccountManager.getAccount(p.readQ(), true);
            if (account3 == null)
              break;
            FriendState friendState = num3 == 1 ? FriendState.Online : FriendState.Offline;
            if (num2 == 0)
            {
              int index = -1;
              Friend friend = account3.FriendSystem.GetFriend(account2.player_id, out index);
              if (index == -1 || friend == null || friend.state != 0)
                break;
              account3.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(FriendChangeState.Update, friend, friendState, index));
              break;
            }
            account3.SendPacket((SendPacket) new PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK(account2, friendState));
            break;
          case 13:
            long id1 = p.readQ();
            byte num4 = p.readC();
            byte[] data = p.readB((int) p.readUH());
            PointBlank.Game.Data.Model.Account account4 = AccountManager.getAccount(id1, true);
            if (account4 == null)
              break;
            if (num4 == (byte) 0)
            {
              account4.SendPacket(data);
              break;
            }
            account4.SendCompletePacket(data);
            break;
          case 15:
            int id2 = p.readD();
            int num5 = p.readD();
            GameServerModel server = ServersXml.getServer(id2);
            if (server == null)
              break;
            server._LastCount = num5;
            break;
          case 16:
            ClanSync.Load(p);
            break;
          case 17:
            FriendSync.Load(p);
            break;
          case 18:
            InventorySync.Load(p);
            break;
          case 19:
            PlayerSync.Load(p);
            break;
          case 20:
            ServerWarning.LoadGMWarning(p);
            break;
          case 21:
            ClanServersSync.Load(p);
            break;
          case 22:
            ServerWarning.LoadShopRestart(p);
            break;
          case 23:
            ServerWarning.LoadServerUpdate(p);
            break;
          case 24:
            ServerWarning.LoadShutdown(p);
            break;
          case 31:
            EventLoader.ReloadEvent((int) p.readC());
            Logger.warning("GameSync - Reloaded event.");
            break;
          case 32:
            ServerConfigSyncer.GenerateConfig((int) p.readC());
            Logger.warning("GameSync - Reset (DB) settings.");
            break;
          default:
            Logger.warning("GameSync - Connection opcode not found: " + (object) num1);
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.error("GameSync - Opcode: " + (object) num1 + "\r\n" + ex.ToString());
        if (p == null)
          return;
        Logger.error("Buffer: " + BitConverter.ToString(p.getBuffer()));
      }
    }

    public static void SendUDPPlayerSync(PointBlank.Game.Data.Model.Room room, Slot slot, CouponEffects effects, int type)
    {
      try
      {
        using (SendGPacket pk = new SendGPacket())
        {
          if (room == null || slot == null || room.UdpServer.Connection == null)
            return;
          pk.writeH((short) 1);
          pk.writeD(room.UniqueRoomId);
          pk.writeD(room.Seed);
          pk.writeQ(room.StartTick);
          pk.writeC((byte) type);
          pk.writeC((byte) room.rounds);
          pk.writeC((byte) slot._id);
          pk.writeC((byte) slot.spawnsCount);
          pk.writeC(BitConverter.GetBytes(slot._playerId)[0]);
          if (type == 0 || type == 2)
            GameSync.WriteCharaInfo(pk, room, slot, effects);
          GameSync.SendPacket(pk.mstream.ToArray(), room.UdpServer.Connection);
        }
      }
      catch
      {
      }
    }

    private static void WriteCharaInfo(
      SendGPacket pk,
      PointBlank.Game.Data.Model.Room room,
      Slot slot,
      CouponEffects effects)
    {
      try
      {
        int id = room.room_type == RoomType.Boss || room.room_type == RoomType.CrossCounter ? (room.rounds == 1 && slot._team == 1 || room.rounds == 2 && slot._team == 0 ? (room.rounds == 2 ? slot._equip._red : slot._equip._blue) : (room.TRex != slot._id ? slot._equip._dino : -1)) : (slot._team == 0 ? slot._equip._red : slot._equip._blue);
        int num = 0;
        if (effects.HasFlag((System.Enum) CouponEffects.HP5))
          num += 5;
        if (effects.HasFlag((System.Enum) CouponEffects.HP10))
          num += 10;
        if (id == -1)
        {
          pk.writeC(byte.MaxValue);
          pk.writeH(ushort.MaxValue);
        }
        else
        {
          pk.writeC((byte) ComDiv.getIdStatics(id, 2));
          pk.writeH((short) ComDiv.getIdStatics(id, 3));
        }
        pk.writeC((byte) num);
        pk.writeC(effects.HasFlag((System.Enum) CouponEffects.C4SpeedKit));
      }
      catch
      {
      }
    }

    public static void SendUDPRoundSync(PointBlank.Game.Data.Model.Room room)
    {
      try
      {
        using (SendGPacket sendGpacket = new SendGPacket())
        {
          if (room == null)
            return;
          sendGpacket.writeH((short) 3);
          sendGpacket.writeD(room.UniqueRoomId);
          sendGpacket.writeD(room.Seed);
          sendGpacket.writeC((byte) room.rounds);
          GameSync.SendPacket(sendGpacket.mstream.ToArray(), room.UdpServer.Connection);
        }
      }
      catch
      {
      }
    }

    public static GameServerModel GetServer(AccountStatus status)
    {
      return GameSync.GetServer((int) status.serverId);
    }

    public static GameServerModel GetServer(int serverId)
    {
      if (serverId == (int) byte.MaxValue || serverId == GameConfig.serverId)
        return (GameServerModel) null;
      return ServersXml.getServer(serverId);
    }

    public static void UpdateGSCount(int serverId)
    {
      try
      {
        if ((DateTime.Now - GameSync.LastSyncCount).TotalSeconds < 5.0)
          return;
        GameSync.LastSyncCount = DateTime.Now;
        int num = 0;
        for (int index = 0; index < ChannelsXml._channels.Count; ++index)
        {
          Channel channel = ChannelsXml._channels[index];
          num += channel._players.Count;
        }
        for (int index = 0; index < ServersXml._servers.Count; ++index)
        {
          GameServerModel server = ServersXml._servers[index];
          if (server._id == serverId)
          {
            server._LastCount = num;
          }
          else
          {
            using (SendGPacket sendGpacket = new SendGPacket())
            {
              sendGpacket.writeH((short) 15);
              sendGpacket.writeD(serverId);
              sendGpacket.writeD(num);
              GameSync.SendPacket(sendGpacket.mstream.ToArray(), server.Connection);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.warning("[GameSync.UpdateGSCount] " + ex.ToString());
      }
    }

    public static void SendBytes(long playerId, SendPacket sp, int serverId)
    {
      if (sp == null)
        return;
      GameServerModel server = GameSync.GetServer(serverId);
      if (server == null)
        return;
      byte[] bytes = sp.GetBytes("GameSync.SendBytes");
      using (SendGPacket sendGpacket = new SendGPacket())
      {
        sendGpacket.writeH((short) 13);
        sendGpacket.writeQ(playerId);
        sendGpacket.writeC((byte) 0);
        sendGpacket.writeH((ushort) bytes.Length);
        sendGpacket.writeB(bytes);
        GameSync.SendPacket(sendGpacket.mstream.ToArray(), server.Connection);
      }
    }

    public static void SendBytes(long playerId, byte[] buffer, int serverId)
    {
      if (buffer.Length == 0)
        return;
      GameServerModel server = GameSync.GetServer(serverId);
      if (server == null)
        return;
      using (SendGPacket sendGpacket = new SendGPacket())
      {
        sendGpacket.writeH((short) 13);
        sendGpacket.writeQ(playerId);
        sendGpacket.writeC((byte) 0);
        sendGpacket.writeH((ushort) buffer.Length);
        sendGpacket.writeB(buffer);
        GameSync.SendPacket(sendGpacket.mstream.ToArray(), server.Connection);
      }
    }

    public static void SendCompleteBytes(long playerId, byte[] buffer, int serverId)
    {
      if (buffer.Length == 0)
        return;
      GameServerModel server = GameSync.GetServer(serverId);
      if (server == null)
        return;
      using (SendGPacket sendGpacket = new SendGPacket())
      {
        sendGpacket.writeH((short) 13);
        sendGpacket.writeQ(playerId);
        sendGpacket.writeC((byte) 1);
        sendGpacket.writeH((ushort) buffer.Length);
        sendGpacket.writeB(buffer);
        GameSync.SendPacket(sendGpacket.mstream.ToArray(), server.Connection);
      }
    }

    public static void SendPacket(byte[] data, IPEndPoint ip)
    {
      GameSync.udp.Send(data, data.Length, ip);
    }

    public static void genDeath(PointBlank.Game.Data.Model.Room room, Slot killer, FragInfos kills, bool isSuicide)
    {
      bool isBotMode = room.isBotMode();
      int score;
      RoomDeath.RegistryFragInfos(room, killer, out score, isBotMode, isSuicide, kills);
      if (isBotMode)
      {
        killer.Score += killer.killsOnLife + (int) room.IngameAiLevel + score;
        if (killer.Score > (int) ushort.MaxValue)
        {
          killer.Score = (int) ushort.MaxValue;
          Logger.warning("[PlayerId: " + (object) killer._id + "] reached the maximum score of the BOT.");
        }
        kills.Score = killer.Score;
      }
      else
      {
        killer.Score += score;
        AllUtils.CompleteMission(room, killer, kills, MissionType.NA, 0);
        kills.Score = score;
      }
      using (PROTOCOL_BATTLE_DEATH_ACK protocolBattleDeathAck = new PROTOCOL_BATTLE_DEATH_ACK(room, kills, killer, isBotMode))
        room.SendPacketToPlayers((SendPacket) protocolBattleDeathAck, SlotState.BATTLE, 0);
      RoomDeath.EndBattleByDeath(room, killer, isBotMode, isSuicide);
    }
  }
}
