using PointBlank.Core;
using PointBlank.Core.Filters;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Sync;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PointBlank.Game
{
  public class Programm
  {
    public static void Main(string[] args)
    {
      string str1 = ComDiv.GetLinkerTime(Assembly.GetExecutingAssembly(), (TimeZoneInfo) null).ToString("dd/MM/yyyy HH:mm");
      Console.Title = "Point Blank - Game";
      Logger.StartedFor = "Game";
      Logger.checkDirectorys();
      Console.Clear();
      Logger.title("________________________________________________________________________________");
      Logger.title("                                                                               ");
      Logger.title("                                                                               ");
      Logger.title("                                POINT BLANK GAME                               ");
      Logger.title("                                                                               ");
      Logger.title("                                                                               ");
      Logger.title("_______________________________ " + str1 + " _______________________________");
      GameConfig.Load();
      BasicInventoryXml.Load();
      ServerConfigSyncer.GenerateConfig(GameConfig.configId);
      ServersXml.Load();
      ChannelsXml.Load(GameConfig.serverId);
      EventLoader.LoadAll();
      TitlesXml.Load();
      TitleAwardsXml.Load();
      ClanManager.Load();
      NickFilter.Load();
      MissionCardXml.LoadBasicCards(1);
      RankXml.Load();
      BattleServerXml.Load();
      RankXml.LoadAwards();
      ClanRankXml.Load();
      MissionAwardsXml.Load();
      MissionsXml.Load();
      Translation.Load();
      ShopManager.Load(1);
      MapsXml.Load();
      RandomBoxXml.LoadBoxes();
      CouponEffectManager.LoadCouponFlags();
      ICafeManager.GetList();
      GameRuleManager.getGameRules(GameConfig.ruleId);
      GameSync.Start();
      bool flag = GameManager.Start();
      Logger.info("Text Encode: " + Config.EncodeText.EncodingName);
      Logger.info("Mode: " + (GameConfig.isTestMode ? "Test" : "Public"));
      Logger.debug(Programm.StartSuccess());
      if (flag)
        PointBlank.Game.Game.Update();
      while (true)
      {
        string text;
        do
        {
          do
          {
            text = Console.ReadLine();
          }
          while (text.StartsWith("test"));
          if (text.StartsWith("msg "))
          {
            string str2 = "";
            try
            {
              using (PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK messageAnnounceAck = new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(text.Substring(4)))
                str2 = "Send message to: " + (object) GameManager.SendPacketToAllClients((SendPacket) messageAnnounceAck) + " players.";
            }
            catch
            {
              str2 = "Command Error.";
            }
            Logger.console(str2);
            Logger.LogConsole(text, str2);
          }
          else if (text.StartsWith("ban "))
          {
            string str2;
            try
            {
              Account account = AccountManager.getAccount(long.Parse(text.Substring(4)), 0);
              if (account == null)
                str2 = "Invalid Player.";
              else if (account.access != AccessLevel.Banned)
              {
                if (ComDiv.updateDB("accounts", "access_level", (object) -1, "player_id", (object) account.player_id))
                {
                  BanManager.SaveAutoBan(account.player_id, account.login, account.player_name, "Cheating Use", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), account.PublicIP.ToString(), "Ban from Console");
                  using (PROTOCOL_LOBBY_CHATTING_ACK lobbyChattingAck = new PROTOCOL_LOBBY_CHATTING_ACK("Server", 0U, 0, true, "Player Banned [" + account.player_name + "] Permanent - Cheating Use"))
                    GameManager.SendPacketToAllClients((SendPacket) lobbyChattingAck);
                  account.access = AccessLevel.Banned;
                  account.SendPacket((SendPacket) new PROTOCOL_AUTH_ACCOUNT_KICK_ACK(2), false);
                  account.Close(1000, true);
                  str2 = "Ban Success.";
                }
                else
                  str2 = "Ban Player Failed.";
              }
              else
                str2 = "Players are already banned.";
            }
            catch
            {
              str2 = "Command Error.";
            }
            Logger.console(str2);
            Logger.LogConsole(text, str2);
          }
          else if (text.StartsWith("unban "))
          {
            string str2;
            try
            {
              Account account = AccountManager.getAccount(long.Parse(text.Substring(6)), 0);
              if (account == null)
                str2 = "Invalid Player.";
              else if (account.access == AccessLevel.Banned || account.ban_obj_id > 0L)
              {
                if (ComDiv.updateDB("accounts", "access_level", (object) 0, "player_id", (object) account.player_id))
                {
                  if (ComDiv.updateDB("accounts", "ban_obj_id", (object) 0, "player_id", (object) account.player_id))
                    ComDiv.deleteDB("auto_ban", "player_id", (object) account.player_id);
                  account.access = AccessLevel.Normal;
                  str2 = "Unban Success.";
                }
                else
                  str2 = "Unban Player Failed.";
              }
              else
                str2 = "Player not being banned.";
            }
            catch
            {
              str2 = "Command Error.";
            }
            Logger.console(str2);
            Logger.LogConsole(text, str2);
          }
          else if (text.StartsWith("kickall"))
          {
            string str2;
            try
            {
              int num = 0;
              using (PROTOCOL_AUTH_ACCOUNT_KICK_ACK authAccountKickAck = new PROTOCOL_AUTH_ACCOUNT_KICK_ACK(0))
              {
                if (GameManager._socketList.Count > 0)
                {
                  byte[] completeBytes = authAccountKickAck.GetCompleteBytes("Console.KickAll");
                  foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
                  {
                    Account player = gameClient._player;
                    if (player != null && player._isOnline && player.access <= AccessLevel.Streamer)
                    {
                      player.SendCompletePacket(completeBytes);
                      player.Close(1000, true);
                      ++num;
                    }
                  }
                }
              }
              str2 = "Kick " + (object) num + " players.";
            }
            catch
            {
              str2 = "Command Error.";
            }
            Logger.console(str2);
            Logger.LogConsole(text, str2);
          }
          else if (text.StartsWith("reload_shop"))
          {
            string str2;
            try
            {
              ShopManager.Reset();
              ShopManager.Load(1);
              foreach (GameClient gameClient in (IEnumerable<GameClient>) GameManager._socketList.Values)
              {
                Account player = gameClient._player;
                if (player != null && player._isOnline)
                {
                  for (int index = 0; index < ShopManager.ShopDataItems.Count; ++index)
                  {
                    ShopData shopDataItem = ShopManager.ShopDataItems[index];
                    player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEMLIST_ACK(shopDataItem, ShopManager.TotalItems));
                  }
                  for (int index = 0; index < ShopManager.ShopDataGoods.Count; ++index)
                  {
                    ShopData shopDataGood = ShopManager.ShopDataGoods[index];
                    player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_GOODSLIST_ACK(shopDataGood, ShopManager.TotalGoods));
                  }
                  for (int index = 0; index < ShopManager.ShopDataItemRepairs.Count; ++index)
                  {
                    ShopData shopDataItemRepair = ShopManager.ShopDataItemRepairs[index];
                    player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_REPAIRLIST_ACK(shopDataItemRepair, ShopManager.TotalRepairs));
                  }
                  if (player.pc_cafe == 0)
                  {
                    for (int index = 0; index < ShopManager.ShopDataMt1.Count; ++index)
                    {
                      ShopData data = ShopManager.ShopDataMt1[index];
                      player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK(data, ShopManager.TotalMatching1));
                    }
                  }
                  else
                  {
                    for (int index = 0; index < ShopManager.ShopDataMt2.Count; ++index)
                    {
                      ShopData data = ShopManager.ShopDataMt2[index];
                      player.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK(data, ShopManager.TotalMatching2));
                    }
                  }
                  player.SendPacket((SendPacket) new PROTOCOL_SHOP_GET_SAILLIST_ACK(true));
                }
              }
              str2 = "Reload Shop Success.";
            }
            catch
            {
              str2 = "Command Error.";
            }
            Logger.console(str2);
            Logger.LogConsole(text, str2);
          }
          else if (text.StartsWith("reload_event"))
          {
            string str2;
            try
            {
              EventLoader.ReloadAll();
              str2 = "Reloaded Event Success.";
            }
            catch
            {
              str2 = "Command Error.";
            }
            Logger.console(str2);
            Logger.LogConsole(text, str2);
          }
        }
        while (!text.StartsWith("reload_rule"));
        string str3;
        try
        {
          GameRuleManager.Reload();
          str3 = "Reloaded GameRule Success.";
        }
        catch
        {
          str3 = "Command Error.";
        }
        Logger.console(str3);
        Logger.LogConsole(text, str3);
      }
    }

    private static string StartSuccess()
    {
      if (Logger.erro)
        return "Start failed.";
      return "Active Server. (" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + ")";
    }
  }
}
