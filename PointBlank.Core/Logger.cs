using System;
using System.IO;

namespace PointBlank.Core
{
  public static class Logger
  {
    private static string Date = DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss");
    public static string StartedFor = "None";
    private static object Sync = new object();
    public static bool erro;

    public static void LogLogin(string text)
    {
      try
      {
        Logger.save("[Date: " + DateTime.Now.ToString("dd/MM/yy HH:mm") + "] Login: " + text, "Login");
      }
      catch
      {
      }
    }

    public static void LogProblems(string text, string problemInfo)
    {
      try
      {
        Logger.save("[Data: " + DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + "]" + text, problemInfo);
      }
      catch
      {
      }
    }

    public static void LogCMD(string text)
    {
      try
      {
        Logger.save(text, "Command");
      }
      catch
      {
      }
    }

    public static void LogConsole(string text, string Result)
    {
      try
      {
        Logger.save("[" + DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + "] Command: " + text + " Result: " + Result, "Console");
      }
      catch
      {
      }
    }

    public static void LogHack(string text)
    {
      try
      {
        Logger.save("[" + DateTime.Now.ToString("yy/MM/dd HH:mm:ss") + "]: " + text, "Hack");
      }
      catch
      {
      }
    }

    public static void title(string text)
    {
      Logger.write(text, ConsoleColor.Green);
    }

    public static void info(string text)
    {
      Logger.write(DateTime.Now.ToString("HH:mm:ss ") + "[Info] " + text, ConsoleColor.Gray);
    }

    public static void warning(string text)
    {
      Logger.write(DateTime.Now.ToString("HH:mm:ss ") + "[Warning] " + text, ConsoleColor.Yellow);
    }

    public static void console(string text)
    {
      Logger.write(DateTime.Now.ToString("HH:mm:ss ") + "[Console] " + text, ConsoleColor.Cyan);
    }

    public static void error(string text)
    {
      Logger.write(DateTime.Now.ToString("HH:mm:ss ") + "[Error] " + text, ConsoleColor.Red);
    }

    public static void debug(string text)
    {
      Logger.write(DateTime.Now.ToString("HH:mm:ss ") + "[Debug] " + text, ConsoleColor.Green);
    }

    public static void send(string text)
    {
      Logger.write(text, ConsoleColor.Gray);
    }

    public static void unkhown_packet(string text)
    {
      Logger.write(text, ConsoleColor.Green);
    }

    private static void write(string text, ConsoleColor color)
    {
      try
      {
        lock (Logger.Sync)
        {
          Console.ForegroundColor = color;
          Console.WriteLine(text);
          Logger.save(text, Logger.StartedFor);
        }
      }
      catch
      {
      }
    }

    public static void checkDirectorys()
    {
      try
      {
        if (Logger.StartedFor == "Auth")
        {
          if (!Directory.Exists("Logs/Auth"))
            Directory.CreateDirectory("Logs/Auth");
          if (Directory.Exists("Logs/Login"))
            return;
          Directory.CreateDirectory("Logs/Login");
        }
        else
        {
          if (!Directory.Exists("Logs/Command"))
            Directory.CreateDirectory("Logs/Command");
          if (!Directory.Exists("Logs/Console"))
            Directory.CreateDirectory("Logs/Console");
          if (!Directory.Exists("Logs/Game"))
            Directory.CreateDirectory("Logs/Game");
          if (!Directory.Exists("Logs/Hack"))
            Directory.CreateDirectory("Logs/Hack");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    public static void save(string text, string type)
    {
      using (FileStream fileStream = new FileStream("Logs/" + type + "/" + Logger.Date + ".log", FileMode.Append))
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream))
        {
          try
          {
            if (streamWriter != null)
              streamWriter.WriteLine(text);
          }
          catch
          {
          }
          streamWriter.Flush();
          streamWriter.Close();
          fileStream.Flush();
          fileStream.Close();
        }
      }
    }
  }
}
