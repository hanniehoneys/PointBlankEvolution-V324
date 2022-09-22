using PointBlank.Auth.Data.Configs;
using PointBlank.Auth.Data.Sync;
using PointBlank.Auth.Data.Xml;
using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using System;
using System.Reflection;

namespace PointBlank.Auth
{
  public class Programm
  {
    private static void Main(string[] args)
    {
      string str1 = ComDiv.GetLinkerTime(Assembly.GetExecutingAssembly(), (TimeZoneInfo) null).ToString("dd/MM/yyyy HH:mm");
      Console.Title = "Point Blank - Auth";
      Logger.StartedFor = "Auth";
      Logger.checkDirectorys();
      Console.Clear();
      Logger.title("________________________________________________________________________________");
      Logger.title("                                                                               ");
      Logger.title("                                                                               ");
      Logger.title("                                POINT BLANK AUTH                               ");
      Logger.title("                                                                               ");
      Logger.title("                                                                               ");
      Logger.title("_______________________________ " + str1 + " _______________________________");
      AuthConfig.Load();
      ServerConfigSyncer.GenerateConfig(AuthConfig.configId);
      EventLoader.LoadAll();
      BasicInventoryXml.Load();
      ServersXml.Load();
      ChannelsXml.Load(AuthConfig.serverId);
      MissionCardXml.LoadBasicCards(2);
      MapsXml.Load();
      ShopManager.Load(1);
      ShopManager.Load(2);
      RankXml.Load();
      RankXml.LoadAwards();
      CouponEffectManager.LoadCouponFlags();
      QuickStartXml.Load();
      ICafeManager.GetList();
      MissionsXml.Load();
      AuthSync.Start();
      bool flag = AuthManager.Start();
      Logger.info("Text Encode: " + Config.EncodeText.EncodingName);
      Logger.info("Mode: " + (AuthConfig.isTestMode ? "Test" : "Public"));
      Logger.debug(Programm.StartSuccess());
      if (flag)
        PointBlank.Auth.Auth.Update();
      while (true)
      {
        string text;
        do
        {
          text = Console.ReadLine();
        }
        while (!text.StartsWith("reload_event"));
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

    private static string StartSuccess()
    {
      if (Logger.erro)
        return "Startup failed.";
      return "Active Server. (" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + ")";
    }
  }
}
