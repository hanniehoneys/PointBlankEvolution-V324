using PointBlank.Battle.Data.Configs;
using PointBlank.Battle.Data.Enums;
using PointBlank.Battle.Data.Models;
using PointBlank.Battle.Data.Sync.Client;
using PointBlank.Battle.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PointBlank.Battle.Data.Sync
{
  public class BattleSync
  {
    private static UdpClient udp;

    public static void Start()
    {
      try
      {
        udp = new UdpClient(BattleConfig.syncPort);
        uint IOC_IN = 0x80000000;
        uint IOC_VENDOR = 0x18000000;
        uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
        udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
        new Thread(Read).Start();
        //Logger.debug("Server sync port: " + Config.syncPort);
      }
        catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
    }

    public static void Read()
    {
      try
      {
        BattleSync.udp.BeginReceive(new AsyncCallback(BattleSync.AsyncCallback), (object) null);
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    private static void AsyncCallback(IAsyncResult res)
    {
      IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 8000);
      byte[] buffer = BattleSync.udp.EndReceive(res, ref remoteEP);
      new Thread(new ThreadStart(BattleSync.Read)).Start();
      if (buffer.Length < 2)
        return;
      BattleSync.LoadPacket(buffer);
    }

    private static void LoadPacket(byte[] buffer)
    {
      ReceivePacket p = new ReceivePacket(buffer);
      switch (p.readH())
      {
        case 1:
          RespawnSync.Load(p);
          break;
        case 2:
          RemovePlayerSync.Load(p);
          break;
        case 3:
          uint UniqueRoomId = p.readUD();
          uint Seed = p.readUD();
          int num = (int) p.readC();
          Room room = RoomsManager.getRoom(UniqueRoomId, Seed);
          if (room != null)
            room.ServerRound = num;
          break;
      }
    }

    public static void SendPortalPass(Room room, Player pl, int portalIdx)
    {
      if (room.RoomType != ROOM_STATE_TYPE.Boss)
        return;
      pl.Life = pl.MaxLife;
      using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
      {
        using (SendPacket sendPacket = new SendPacket())
        {
          sendPacket.writeH((short) 1);
          sendPacket.writeH((short) room.RoomId);
          sendPacket.writeH((short) room.ChannelId);
          sendPacket.writeC((byte) pl.Slot);
          sendPacket.writeC((byte) portalIdx);
          BattleSync.SendData(room, socket, sendPacket.mstream.ToArray());
        }
      }
    }

    public static void SendDeathSync(
      Room room,
      Player killer,
      int objId,
      int weaponId,
      List<DeathServerData> deaths)
    {
      using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
      {
        using (SendPacket sendPacket = new SendPacket())
        {
          sendPacket.writeH((short) 3);
          sendPacket.writeH((short) room.RoomId);
          sendPacket.writeH((short) room.ChannelId);
          sendPacket.writeC((byte) killer.Slot);
          sendPacket.writeC((byte) objId);
          sendPacket.writeD(weaponId);
          sendPacket.writeTVector(killer.Position);
          sendPacket.writeC((byte) deaths.Count);
          for (int index = 0; index < deaths.Count; ++index)
          {
            DeathServerData death = deaths[index];
            sendPacket.writeC((byte) AllUtils.getIdStatics(weaponId, 2));
            sendPacket.writeC((byte) ((int) death.DeathType * 16 + death.Player.Slot));
            sendPacket.writeTVector(death.Player.Position);
            sendPacket.writeC((byte) death.Assist);
          }
          BattleSync.SendData(room, socket, sendPacket.mstream.ToArray());
        }
      }
    }

    public static void SendBombSync(Room room, Player pl, int type, int bombArea)
    {
      using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
      {
        using (SendPacket sendPacket = new SendPacket())
        {
          sendPacket.writeH((short) 2);
          sendPacket.writeH((short) room.RoomId);
          sendPacket.writeH((short) room.ChannelId);
          sendPacket.writeC((byte) type);
          sendPacket.writeC((byte) pl.Slot);
          if (type == 0)
          {
            sendPacket.writeC((byte) bombArea);
            sendPacket.writeTVector(pl.Position);
            room.BombPosition = pl.Position;
          }
          BattleSync.SendData(room, socket, sendPacket.mstream.ToArray());
        }
      }
    }

    public static void SendHitMarkerSync(
      Room room,
      Player pl,
      int deathType,
      int hitEnum,
      int damage)
    {
      using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
      {
        using (SendPacket sendPacket = new SendPacket())
        {
          sendPacket.writeH((short) 4);
          sendPacket.writeH((short) room.RoomId);
          sendPacket.writeH((short) room.ChannelId);
          sendPacket.writeC((byte) pl.Slot);
          sendPacket.writeC((byte) deathType);
          sendPacket.writeC((byte) hitEnum);
          sendPacket.writeH((short) damage);
          BattleSync.SendData(room, socket, sendPacket.mstream.ToArray());
        }
      }
    }

    public static void SendSabotageSync(Room room, Player pl, int damage, int ultraSYNC)
    {
      using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
      {
        using (SendPacket sendPacket = new SendPacket())
        {
          sendPacket.writeH((short) 5);
          sendPacket.writeH((short) room.RoomId);
          sendPacket.writeH((short) room.ChannelId);
          sendPacket.writeC((byte) pl.Slot);
          sendPacket.writeH((ushort) room.Bar1);
          sendPacket.writeH((ushort) room.Bar2);
          sendPacket.writeC((byte) ultraSYNC);
          sendPacket.writeH((ushort) damage);
          BattleSync.SendData(room, socket, sendPacket.mstream.ToArray());
        }
      }
    }

    private static void SendData(Room room, Socket socket, byte[] data)
    {
      if (!BattleConfig.sendInfoToServ)
        return;
      socket.SendTo(data, (EndPoint) room.GameServer.Connection);
    }
  }
}
