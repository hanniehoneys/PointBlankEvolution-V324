using System;
using System.Collections.Generic;
using System.IO;

namespace PointBlank.Battle.Data
{
  public class ConfigFile
  {
    private FileInfo File;
    private SortedList<string, string> _topics;

    public ConfigFile(string path)
    {
      try
      {
        this.File = new FileInfo(path);
        this._topics = new SortedList<string, string>();
        this.LoadStrings();
      }
      catch (Exception ex)
      {
        Logger.error("[ConfigFile] " + ex.ToString());
      }
    }

    private void LoadStrings()
    {
      try
      {
        using (StreamReader streamReader = new StreamReader(this.File.FullName))
        {
          while (!streamReader.EndOfStream)
          {
            string str = streamReader.ReadLine();
            if (str.Length != 0 && !str.StartsWith(";") && !str.StartsWith("["))
            {
              string[] strArray = str.Split('=');
              this._topics.Add(strArray[0], strArray[1]);
            }
          }
          streamReader.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    public float readFloat(string value, float defaultprop)
    {
      float num;
      try
      {
        num = float.Parse(this._topics[value]);
      }
      catch
      {
        this.Error(value);
        return defaultprop;
      }
      return num;
    }

    public bool readBoolean(string value, bool defaultprop)
    {
      bool flag;
      try
      {
        flag = bool.Parse(this._topics[value]);
      }
      catch
      {
        this.Error(value);
        return defaultprop;
      }
      return flag;
    }

    public long readInt64(string value, long defaultprop)
    {
      long num;
      try
      {
        num = long.Parse(this._topics[value]);
      }
      catch
      {
        this.Error(value);
        return defaultprop;
      }
      return num;
    }

    public ulong readUInt64(string value, ulong defaultprop)
    {
      ulong num;
      try
      {
        num = ulong.Parse(this._topics[value]);
      }
      catch
      {
        this.Error(value);
        return defaultprop;
      }
      return num;
    }

    public int readInt32(string value, int defaultprop)
    {
      int num;
      try
      {
        num = int.Parse(this._topics[value]);
      }
      catch
      {
        this.Error(value);
        return defaultprop;
      }
      return num;
    }

    public uint readUInt32(string value, uint defaultprop)
    {
      uint num;
      try
      {
        num = uint.Parse(this._topics[value]);
      }
      catch
      {
        this.Error(value);
        return defaultprop;
      }
      return num;
    }

    public ushort readUInt16(string value, ushort defaultprop)
    {
      ushort num;
      try
      {
        num = ushort.Parse(this._topics[value]);
      }
      catch
      {
        this.Error(value);
        return defaultprop;
      }
      return num;
    }

    public byte readByte(string value, byte defaultprop)
    {
      byte num;
      try
      {
        num = byte.Parse(this._topics[value]);
      }
      catch
      {
        this.Error(value);
        return defaultprop;
      }
      return num;
    }

    public string readString(string value, string defaultprop)
    {
      string topic;
      try
      {
        topic = this._topics[value];
      }
      catch
      {
        this.Error(value);
        return defaultprop;
      }
      return topic == null ? defaultprop : topic;
    }

    private void Error(string parameter)
    {
      Logger.warning("ConfigFile Parameter failure: " + parameter);
    }
  }
}
