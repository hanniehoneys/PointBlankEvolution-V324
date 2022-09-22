using PointBlank.Core;
using PointBlank.Game.Data.Configs;

namespace PointBlank.Game.Data.Chat
{
  public static class ChangeServerMode
  {
    public static string EnableTestMode()
    {
      if (GameConfig.isTestMode)
        return Translation.GetLabel("AlreadyTestModeOn");
      GameConfig.isTestMode = true;
      return Translation.GetLabel("TestModeOn");
    }

    public static string EnablePublicMode()
    {
      if (!GameConfig.isTestMode)
        return Translation.GetLabel("AlreadyTestModeOff");
      GameConfig.isTestMode = false;
      return Translation.GetLabel("TestModeOff");
    }
  }
}
