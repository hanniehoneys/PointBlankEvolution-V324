using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using System.Text;

namespace PointBlank.Game.Data.Configs
{
  public static class GameConfig
  {
    public static string passw;
    public static string gameIp;
    public static bool isTestMode;
    public static bool debugMode;
    public static bool winCashPerBattle;
    public static bool showCashReceiveWarn;
    public static bool AutoBan;
    public static UdpState udpType;
    public static float maxClanPoints;
    public static int serverId;
    public static int configId;
    public static int ruleId;
    public static int maxBattleLatency;
    public static int maxRepeatLatency;
    public static int syncPort;
    public static int maxActiveClans;
    public static int minRankVote;
    public static int maxNickSize;
    public static int minNickSize;
    public static int maxBattleXP;
    public static int maxBattleGP;
    public static int maxBattleMY;
    public static int maxChannelPlayers;
    public static int gamePort;
    public static int minCreateGold;
    public static int minCreateRank;
    public static int ICafePoint;
    public static int ICafeExp;

    public static void Load()
    {
      ConfigFile configFile = new ConfigFile("Config/Game.ini");
      Config.dbHost = configFile.readString("Host", "localhost");
      Config.dbName = configFile.readString("Name", "");
      Config.dbUser = configFile.readString("User", "root");
      Config.dbPass = configFile.readString("Pass", "");
      Config.dbPort = configFile.readInt32("Port", 0);
      GameConfig.serverId = configFile.readInt32("ServerId", -1);
      GameConfig.configId = configFile.readInt32("ConfigId", 0);
      GameConfig.gameIp = configFile.readString("GameIp", "127.0.0.1");
      GameConfig.gamePort = configFile.readInt32("GamePort", 39190);
      GameConfig.syncPort = configFile.readInt32("SyncPort", 0);
      GameConfig.debugMode = configFile.readBoolean("Debug", true);
      GameConfig.isTestMode = configFile.readBoolean("Test", true);
      GameConfig.AutoBan = configFile.readBoolean("AutoBan", false);
      Config.EncodeText = Encoding.GetEncoding(configFile.readInt32("EncodingPage", 0));
      GameConfig.winCashPerBattle = configFile.readBoolean("WinCashPerBattle", true);
      GameConfig.showCashReceiveWarn = configFile.readBoolean("ShowCashReceiveWarn", true);
      GameConfig.minCreateRank = configFile.readInt32("MinCreateRank", 15);
      GameConfig.minCreateGold = configFile.readInt32("MinCreatePoint", 7500);
      GameConfig.maxClanPoints = configFile.readFloat("MaxClanPoints", 0.0f);
      GameConfig.passw = configFile.readString("Password", "");
      GameConfig.maxChannelPlayers = configFile.readInt32("MaxChannelPlayers", 100);
      GameConfig.maxBattleXP = configFile.readInt32("MaxBattleXP", 1000);
      GameConfig.maxBattleGP = configFile.readInt32("MaxBattlePoint", 1000);
      GameConfig.maxBattleMY = configFile.readInt32("MaxBattleMY", 1000);
      GameConfig.udpType = (UdpState) configFile.readByte("UdpType", (byte) 1);
      GameConfig.minNickSize = configFile.readInt32("MinNickSize", 0);
      GameConfig.maxNickSize = configFile.readInt32("MaxNickSize", 0);
      GameConfig.minRankVote = configFile.readInt32("MinRankVote", 0);
      GameConfig.maxActiveClans = configFile.readInt32("MaxActiveClans", 0);
      GameConfig.maxBattleLatency = configFile.readInt32("MaxBattleLatency", 0);
      GameConfig.maxRepeatLatency = configFile.readInt32("MaxRepeatLatency", 0);
      GameConfig.ICafePoint = configFile.readInt32("ICafePoint", 2000);
      GameConfig.ICafeExp = configFile.readInt32("ICafeExp", 4000);
      GameConfig.ruleId = configFile.readInt32("RuleId", 0);
    }
  }
}
