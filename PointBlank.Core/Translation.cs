using System.Collections.Generic;
using System.IO;

namespace PointBlank.Core
{
  public static class Translation
  {
    private static SortedList<string, string> strings = new SortedList<string, string>();

    public static void Load()
    {
      foreach (string readAllLine in File.ReadAllLines("Config/Strings.ini"))
      {
        int length = readAllLine.IndexOf("=");
        if (length >= 0)
        {
          string key = readAllLine.Substring(0, length);
          string str = readAllLine.Substring(length + 1);
          Translation.strings.Add(key, str);
        }
      }
    }

    public static string GetLabel(string title)
    {
      try
      {
        string str;
        if (Translation.strings.TryGetValue(title, out str))
          return str.Replace("\\n", '\n'.ToString());
        return title;
      }
      catch
      {
        return title;
      }
    }

    public static string GetLabel(string title, params object[] args)
    {
      return string.Format(Translation.GetLabel(title), args);
    }
  }
}
