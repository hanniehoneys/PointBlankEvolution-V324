using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace PointBlank.Game.Data.Sync.Client
{
  public class ServerWarning
  {
    public static void LoadGMWarning(ReceiveGPacket p)
    {
      string text1 = p.readS((int) p.readC());
      string text2 = p.readS((int) p.readC());
      string msg = p.readS((int) p.readH());
      Account account = AccountManager.getAccount(text1, 0, 0);
      if (account == null || !(account.password == ComDiv.gen5(text2)) || account.access < AccessLevel.GameMaster)
        return;
      int num = 0;
      using (PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK messageAnnounceAck = new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(msg))
        num = GameManager.SendPacketToAllClients((SendPacket) messageAnnounceAck);
      Logger.warning("Message sent to: " + (object) num + " Player: " + msg + " Login: '" + text1 + "' Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
      Logger.LogCMD("Message sent to: " + (object) num + " Player: " + msg + " Login: '" + text1 + "' Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
    }

    public static void LoadShopRestart(ReceiveGPacket p)
    {
      int type = (int) p.readC();
      ShopManager.Reset();
      ShopManager.Load(type);
      Logger.warning("Shop restarted. (Type: " + (object) type + ")");
      Logger.LogCMD("Shop restarted. (Type: " + (object) type + ") Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
    }

    public static void LoadServerUpdate(ReceiveGPacket p)
    {
      int serverId = (int) p.readC();
      ServersXml.UpdateServer(serverId);
      Logger.warning("Server " + (object) serverId + " updated.");
      Logger.LogCMD("Server " + (object) serverId + " updated. Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
    }

    public static void LoadShutdown(ReceiveGPacket p)
    {
      string text1 = p.readS((int) p.readC());
      string text2 = p.readS((int) p.readC());
      Account account = AccountManager.getAccount(text1, 0, 0);
      if (account == null || !(account.password == ComDiv.gen5(text2)) || account.access < AccessLevel.Admin)
        return;
      int num = 0;
      foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
      {
        gameClient._client.Shutdown(SocketShutdown.Both);
        gameClient.Close(5000, false);
        ++num;
      }
      Logger.warning("Offline Players: " + (object) num + ". (" + text1 + ")");
      GameManager.ServerIsClosed = true;
      GameManager.mainSocket.Close(5000);
      Thread.Sleep(5000);
      GameSync.udp.Close();
      foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
        gameClient.Close(0, false);
      Logger.warning("Server was completely shut down.");
      Logger.LogCMD("Shutdown Server: " + (object) num + " players disconnected Login: '" + text1 + "' Date: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
    }
  }
}
