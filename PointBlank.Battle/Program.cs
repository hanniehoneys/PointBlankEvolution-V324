using PointBlank.Battle.Data.Configs;
using PointBlank.Battle.Data.Sync;
using PointBlank.Battle.Data.Xml;
using PointBlank.Battle.Network;
using System;
using System.IO;
using System.Reflection;

namespace PointBlank.Battle
{
  internal class Program
  {
    protected static void Main(string[] args)
    {
      string str = Program.GetLinkerTime(Assembly.GetExecutingAssembly(), (TimeZoneInfo) null).ToString("dd/MM/yyyy HH:mm");
      BattleConfig.Load();
      Logger.checkDirectory();
      Console.Clear();
      Console.Title = "Point Blank - Battle";
      Logger.title("________________________________________________________________________________");
      Logger.title("                                                                               ");
      Logger.title("                                                                               ");
      Logger.title("                               POINT BLANK BATTLE                              ");
      Logger.title("                                                                               ");
      Logger.title("                                                                               ");
      Logger.title("_______________________________ " + str + " _______________________________");
      Logger.info("Server active at " + BattleConfig.hosIp + ":" + (object) BattleConfig.hosPort);
      Logger.info("Synchronize infos to server: " + BattleConfig.sendInfoToServ.ToString());
      Logger.info("Drops Limit: " + (object) BattleConfig.maxDrop);
      Logger.info("Ammo Limit: " + BattleConfig.useMaxAmmoInDrop.ToString());
      Logger.info("Duration C4: (" + (object) BattleConfig.plantDuration + "s/" + (object) BattleConfig.defuseDuration + "s)");
      MapXml.Load();
      CharaXml.Load();
      MeleeExceptionsXml.Load();
      ServersXml.Load();
      BattleSync.Start();
      BattleManager.Connect();
      while (true)
      {
        if (Console.ReadLine().StartsWith("reload_object"))
        {
          MapXml.Reset();
          MapXml.Load();
          Logger.debug("Reload Object Success.");
        }
      }
    }

    public static DateTime GetLinkerTime(Assembly assembly, TimeZoneInfo target = null)
    {
      string location = assembly.Location;
      byte[] buffer = new byte[2048];
      using (FileStream fileStream = new FileStream(location, FileMode.Open, FileAccess.Read))
        fileStream.Read(buffer, 0, 2048);
      int int32 = BitConverter.ToInt32(buffer, 60);
      return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double) BitConverter.ToInt32(buffer, int32 + 8)), target ?? TimeZoneInfo.Local);
    }
  }
}
