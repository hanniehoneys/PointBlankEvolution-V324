using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PointBlank.Auth.Data.Configs
{
  public static class AuthConfig
  {
    public static string authIp;
    public static bool isTestMode;
    public static bool Outpost;
    public static bool AUTO_ACCOUNTS;
    public static bool debugMode;
    public static int syncPort;
    public static int configId;
    public static int maxNickSize;
    public static int minNickSize;
    public static int minTokenSize;
    public static int authPort;
    public static int serverId;
    public static int maxChannelPlayers;
    public static ulong LauncherKey;
    public static List<ClientLocale> GameLocales;

    public static void Load()
    {
      ConfigFile configFile = new ConfigFile("Config/Auth.ini");
      Config.dbHost = configFile.readString("Host", "localhost");
      Config.dbName = configFile.readString("Name", "");
      Config.dbUser = configFile.readString("User", "root");
      Config.dbPass = configFile.readString("Pass", "");
      Config.dbPort = configFile.readInt32("Port", 0);
      AuthConfig.configId = configFile.readInt32("ConfigId", 0);
      AuthConfig.serverId = configFile.readInt32("ServerId", -1);
      AuthConfig.authIp = configFile.readString("AuthIp", "127.0.0.1");
      AuthConfig.authPort = configFile.readInt32("AuthPort", 39190);
      AuthConfig.syncPort = configFile.readInt32("SyncPort", 0);
      AuthConfig.AUTO_ACCOUNTS = configFile.readBoolean("AutoAccounts", false);
      AuthConfig.debugMode = configFile.readBoolean("Debug", true);
      AuthConfig.isTestMode = configFile.readBoolean("Test", true);
      Config.EncodeText = Encoding.GetEncoding(configFile.readInt32("EncodingPage", 0));
      AuthConfig.maxChannelPlayers = configFile.readInt32("MaxChannelPlayers", 100);
      AuthConfig.Outpost = configFile.readBoolean("Outpost", false);
      AuthConfig.LauncherKey = configFile.readUInt64("LauncherKey", 0UL);
      AuthConfig.minNickSize = configFile.readInt32("MinNickSize", 0);
      AuthConfig.maxNickSize = configFile.readInt32("MaxNickSize", 0);
      AuthConfig.minTokenSize = configFile.readInt32("MinTokenSize", 0);
      AuthConfig.GameLocales = new List<ClientLocale>();
      string str1 = configFile.readString("GameLocales", "None");
      char[] chArray = new char[1]{ ',' };
      foreach (string str2 in str1.Split(chArray))
      {
        ClientLocale result;
        Enum.TryParse<ClientLocale>(str2, out result);
        AuthConfig.GameLocales.Add(result);
      }
    }
  }
}
