using PointBlank.Core;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PointBlank.Game
{
  public static class GameManager
  {
    public static ConcurrentDictionary<uint, GameClient> _socketList = new ConcurrentDictionary<uint, GameClient>();
    public static ServerConfig Config;
    public static Socket mainSocket;
    public static bool ServerIsClosed;

    public static bool Start()
    {
      try
      {
        GameManager.Config = ServerConfigSyncer.GenerateConfig(GameConfig.configId);
        GameManager.mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(GameConfig.gameIp), GameConfig.gamePort);
        GameManager.mainSocket.Bind((EndPoint) ipEndPoint);
        GameManager.mainSocket.Listen(10);
        GameManager.mainSocket.BeginAccept(new AsyncCallback(GameManager.AcceptCallback), (object) GameManager.mainSocket);
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
      if (GameManager.ServerIsClosed)
        return;
      Socket asyncState = (Socket) result.AsyncState;
      try
      {
        Socket client = asyncState.EndAccept(result);
        if (client != null)
        {
          GameClient sck = new GameClient(client);
          GameManager.AddSocket(sck);
          if (sck == null)
            Console.WriteLine("Destroyed after failed to add to list.");
          Thread.Sleep(5);
        }
      }
      catch
      {
        Logger.warning("Failed a Client Connection");
      }
      GameManager.mainSocket.BeginAccept(new AsyncCallback(GameManager.AcceptCallback), (object) GameManager.mainSocket);
    }

    public static void AddSocket(GameClient sck)
    {
      if (sck == null)
        return;
      uint num = 0;
      while (num < 100000U)
      {
        uint key = ++num;
        if (!GameManager._socketList.ContainsKey(key) && GameManager._socketList.TryAdd(key, sck))
        {
          sck.SessionId = key;
          sck.Start();
          return;
        }
      }
      sck.Close(500, false);
    }

    public static bool RemoveSocket(GameClient sck)
    {
      if (sck == null || sck.SessionId == 0U || (!GameManager._socketList.ContainsKey(sck.SessionId) || !GameManager._socketList.TryGetValue(sck.SessionId, out sck)))
        return false;
      return GameManager._socketList.TryRemove(sck.SessionId, out sck);
    }

    public static int SendPacketToAllClients(SendPacket packet)
    {
      int num = 0;
      if (GameManager._socketList.Count == 0)
        return num;
      byte[] completeBytes = packet.GetCompleteBytes("GameManager.SendPacketToAllClients");
      foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
      {
        Account player = gameClient._player;
        if (player != null && player._isOnline)
        {
          player.SendCompletePacket(completeBytes);
          ++num;
        }
      }
      return num;
    }

    public static Account SearchActiveClient(long accountId)
    {
      if (GameManager._socketList.Count == 0)
        return (Account) null;
      foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
      {
        Account player = gameClient._player;
        if (player != null && player.player_id == accountId)
          return player;
      }
      return (Account) null;
    }

    public static Account SearchActiveClient(uint sessionId)
    {
      if (GameManager._socketList.Count == 0)
        return (Account) null;
      foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
      {
        if (gameClient._player != null && (int) gameClient.SessionId == (int) sessionId)
          return gameClient._player;
      }
      return (Account) null;
    }

    public static int KickActiveClient(double Hours)
    {
      int num = 0;
      DateTime now = DateTime.Now;
      foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
      {
        Account player = gameClient._player;
        if (player != null && player._room == null && (player.channelId > -1 && !player.IsGM()) && (now - player.LastLobbyEnter).TotalHours >= Hours)
        {
          ++num;
          player.Close(5000, false);
        }
      }
      return num;
    }

    public static int KickCountActiveClient(double Hours)
    {
      int num = 0;
      DateTime now = DateTime.Now;
      foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
      {
        Account player = gameClient._player;
        if (player != null && player._room == null && (player.channelId > -1 && !player.IsGM()) && (now - player.LastLobbyEnter).TotalHours >= Hours)
          ++num;
      }
      return num;
    }
  }
}
