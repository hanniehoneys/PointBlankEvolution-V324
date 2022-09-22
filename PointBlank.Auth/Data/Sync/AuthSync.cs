using PointBlank.Auth.Data.Configs;
using PointBlank.Auth.Data.Managers;
using PointBlank.Auth.Data.Sync.Client;
using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PointBlank.Auth.Data.Sync
{
  public class AuthSync
  {
    private static DateTime LastSyncCount;
    public static UdpClient udp;

    public static void Start()
    {
      try
      {
        udp = new UdpClient(AuthConfig.syncPort);
        uint IOC_IN = 0x80000000;
        uint IOC_VENDOR = 0x18000000;
        uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
        udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
        new Thread(read).Start();
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    public static void read()
    {
      try
      {
        AuthSync.udp.BeginReceive(new AsyncCallback(AuthSync.recv), (object) null);
      }
      catch
      {
      }
    }

    private static void recv(IAsyncResult res)
    {
      if (AuthManager.ServerIsClosed)
        return;
      IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 8000);
      byte[] buffer = AuthSync.udp.EndReceive(res, ref remoteEP);
      Thread.Sleep(5);
      new Thread(new ThreadStart(AuthSync.read)).Start();
      if (buffer.Length < 2)
        return;
      AuthSync.LoadPacket(buffer);
    }

    private static void LoadPacket(byte[] buffer)
    {
      ReceiveGPacket p = new ReceiveGPacket(buffer);
      short num1 = p.readH();
      switch (num1)
      {
        case 11:
          int num2 = (int) p.readC();
          int num3 = (int) p.readC();
          PointBlank.Auth.Data.Model.Account account1 = AccountManager.getInstance().getAccount(p.readQ(), true);
          if (account1 == null)
            break;
          PointBlank.Auth.Data.Model.Account account2 = AccountManager.getInstance().getAccount(p.readQ(), true);
          if (account2 == null)
            break;
          FriendState friendState = num3 == 1 ? FriendState.Online : FriendState.Offline;
          if (num2 == 0)
          {
            int index = -1;
            Friend friend = account2.FriendSystem.GetFriend(account1.player_id, out index);
            if (index == -1 || friend == null)
              break;
            account2.SendPacket((SendPacket) new PROTOCOL_AUTH_FRIEND_INFO_CHANGE_ACK(FriendChangeState.Update, friend, friendState, index));
            break;
          }
          account2.SendPacket((SendPacket) new PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK(account1, friendState));
          break;
        case 13:
          long id1 = p.readQ();
          byte num4 = p.readC();
          byte[] data = p.readB((int) p.readUH());
          PointBlank.Auth.Data.Model.Account account3 = AccountManager.getInstance().getAccount(id1, true);
          if (account3 == null)
            break;
          if (num4 == (byte) 0)
          {
            account3.SendPacket(data);
            break;
          }
          account3.SendCompletePacket(data);
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
        case 19:
          PlayerSync.Load(p);
          break;
        case 20:
          ServerWarning.LoadGMWarning(p);
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
          int index1 = (int) p.readC();
          EventLoader.ReloadEvent(index1);
          Logger.warning("AuthSync Refresh event.");
          Logger.LogCMD("Refresh event; Type: " + (object) index1 + "; Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
          Logger.LogCMD("Refresh event; Type: " + (object) index1 + "; Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
          break;
        case 32:
          ServerConfigSyncer.GenerateConfig((int) p.readC());
          Logger.warning("AuthSync Configuration (Database) Refills.; Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
          Logger.LogCMD("Configuration (Database) Refills.; Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
          break;
        default:
          Logger.warning("AuthSync Connection opcode not found: " + (object) num1);
          break;
      }
    }

    public static void UpdateGSCount(int serverId)
    {
      try
      {
        if ((DateTime.Now - AuthSync.LastSyncCount).TotalSeconds < 2.5)
          return;
        AuthSync.LastSyncCount = DateTime.Now;
        int count = AuthManager._socketList.Count;
        for (int index = 0; index < ServersXml._servers.Count; ++index)
        {
          GameServerModel server = ServersXml._servers[index];
          if (server._id == serverId)
          {
            server._LastCount = count;
          }
          else
          {
            using (SendGPacket sendGpacket = new SendGPacket())
            {
              sendGpacket.writeH((short) 15);
              sendGpacket.writeD(serverId);
              sendGpacket.writeD(count);
              AuthSync.SendPacket(sendGpacket.mstream.ToArray(), server.Connection);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
    }

    public static void SendLoginKickInfo(PointBlank.Auth.Data.Model.Account player)
    {
      int serverId = (int) player._status.serverId;
      switch (serverId)
      {
        case 0:
        case (int) byte.MaxValue:
          player.setOnlineStatus(false);
          break;
        default:
          GameServerModel server = ServersXml.getServer(serverId);
          if (server == null)
            break;
          using (SendGPacket sendGpacket = new SendGPacket())
          {
            sendGpacket.writeH((short) 10);
            sendGpacket.writeQ(player.player_id);
            AuthSync.SendPacket(sendGpacket.mstream.ToArray(), server.Connection);
            break;
          }
      }
    }

    public static void SendPacket(byte[] data, IPEndPoint ip)
    {
      AuthSync.udp.Send(data, data.Length, ip);
    }
  }
}
