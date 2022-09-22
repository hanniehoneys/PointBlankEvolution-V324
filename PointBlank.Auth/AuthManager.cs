using PointBlank.Auth.Data.Configs;
using PointBlank.Auth.Data.Model;
using PointBlank.Core;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PointBlank.Auth
{
  public class AuthManager
  {
    public static ConcurrentDictionary<uint, AuthClient> _socketList = new ConcurrentDictionary<uint, AuthClient>();
    public static List<AuthClient> _loginQueue = new List<AuthClient>();
    public static ServerConfig Config;
    public static Socket mainSocket;
    public static bool ServerIsClosed;

    public static bool Start()
    {
      try
      {
        AuthManager.Config = ServerConfigSyncer.GenerateConfig(AuthConfig.configId);
        AuthManager.mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(AuthConfig.authIp), AuthConfig.authPort);
        AuthManager.mainSocket.Bind((EndPoint) ipEndPoint);
        AuthManager.mainSocket.Listen(10);
        AuthManager.mainSocket.BeginAccept(new AsyncCallback(AuthManager.AcceptCallback), (object) AuthManager.mainSocket);
        return true;
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
        return false;
      }
    }

    private static void AcceptCallback(IAsyncResult result)
    {
      if (AuthManager.ServerIsClosed)
        return;
      Socket asyncState = (Socket) result.AsyncState;
      try
      {
        Socket client = asyncState.EndAccept(result);
        if (client != null)
        {
          AuthClient sck = new AuthClient(client);
          AuthManager.AddSocket(sck);
          if (sck == null)
            Console.WriteLine("Destroyed after failed to add to list.");
          Thread.Sleep(5);
        }
      }
      catch
      {
        Logger.warning("Failed a Client Connection");
      }
      AuthManager.mainSocket.BeginAccept(new AsyncCallback(AuthManager.AcceptCallback), (object) AuthManager.mainSocket);
    }

    public static void AddSocket(AuthClient sck)
    {
      if (sck == null)
        return;
      uint num = 0;
      while (num < 100000U)
      {
        uint key = ++num;
        if (!AuthManager._socketList.ContainsKey(key) && AuthManager._socketList.TryAdd(key, sck))
        {
          sck.SessionId = key;
          sck.Start();
          return;
        }
      }
      sck.Close(500, true);
    }

    public static int EnterQueue(AuthClient sck)
    {
      if (sck == null)
        return -1;
      lock (AuthManager._loginQueue)
      {
        if (AuthManager._loginQueue.Contains(sck))
          return -1;
        AuthManager._loginQueue.Add(sck);
        return AuthManager._loginQueue.IndexOf(sck);
      }
    }

    public static bool RemoveSocket(AuthClient sck)
    {
      if (sck == null || sck.SessionId == 0U || (!AuthManager._socketList.ContainsKey(sck.SessionId) || !AuthManager._socketList.TryGetValue(sck.SessionId, out sck)))
        return false;
      return AuthManager._socketList.TryRemove(sck.SessionId, out sck);
    }

    public static int SendPacketToAllClients(SendPacket packet)
    {
      int num = 0;
      if (AuthManager._socketList.Count > 0)
      {
        byte[] completeBytes = packet.GetCompleteBytes("GameManager.SendPacketToAllClients");
        foreach (AuthClient authClient in (IEnumerable<AuthClient>) AuthManager._socketList.Values)
        {
          Account player = authClient._player;
          if (player != null && player._isOnline)
          {
            player.SendCompletePacket(completeBytes);
            ++num;
          }
        }
      }
      return num;
    }

    public static Account SearchActiveClient(long accountId)
    {
      if (AuthManager._socketList.Count == 0)
        return (Account) null;
      foreach (AuthClient authClient in (IEnumerable<AuthClient>) AuthManager._socketList.Values)
      {
        Account player = authClient._player;
        if (player != null && player.player_id == accountId)
          return player;
      }
      return (Account) null;
    }
  }
}
