using PointBlank.Core;
using System;
using System.Text;

namespace PointBlank.Game.Network
{
  public abstract class ReceivePacket
  {
    private int _offset = 4;
    private byte[] _buffer;
    public GameClient _client;

    protected internal void makeme(GameClient client, byte[] buffer)
    {
      this._client = client;
      this._buffer = buffer;
      this.read();
    }

    protected internal int readD()
    {
      int int32 = BitConverter.ToInt32(this._buffer, this._offset);
      this._offset += 4;
      return int32;
    }

    protected internal uint readUD()
    {
      uint uint32 = BitConverter.ToUInt32(this._buffer, this._offset);
      this._offset += 4;
      return uint32;
    }

    protected internal byte readC()
    {
      return this._buffer[this._offset++];
    }

    protected internal byte readC2()
    {
      try
      {
        return this._buffer[this._offset++];
      }
      catch
      {
        return byte.MaxValue;
      }
    }

    protected internal byte[] readB(int Length)
    {
      byte[] numArray = new byte[Length];
      Array.Copy((Array) this._buffer, this._offset, (Array) numArray, 0, Length);
      this._offset += Length;
      return numArray;
    }

    protected internal short readH()
    {
      short int16 = BitConverter.ToInt16(this._buffer, this._offset);
      this._offset += 2;
      return int16;
    }

    protected internal ushort readUH()
    {
      ushort uint16 = BitConverter.ToUInt16(this._buffer, this._offset);
      this._offset += 2;
      return uint16;
    }

    protected internal double readF()
    {
      double num = BitConverter.ToDouble(this._buffer, this._offset);
      this._offset += 8;
      return num;
    }

    protected internal float readT()
    {
      float single = BitConverter.ToSingle(this._buffer, this._offset);
      this._offset += 4;
      return single;
    }

    protected internal long readQ()
    {
      long int64 = BitConverter.ToInt64(this._buffer, this._offset);
      this._offset += 8;
      return int64;
    }

    protected internal ulong readUQ()
    {
      ulong uint64 = BitConverter.ToUInt64(this._buffer, this._offset);
      this._offset += 8;
      return uint64;
    }

    protected internal string readS(int Length)
    {
      string str = "";
      try
      {
        str = Config.EncodeText.GetString(this._buffer, this._offset, Length);
        int length = str.IndexOf(char.MinValue);
        if (length != -1)
          str = str.Substring(0, length);
        this._offset += Length;
      }
      catch
      {
      }
      return str;
    }

    protected internal string readS(int Length, int CodePage)
    {
      string str = "";
      try
      {
        str = Encoding.GetEncoding(CodePage).GetString(this._buffer, this._offset, Length);
        int length = str.IndexOf(char.MinValue);
        if (length != -1)
          str = str.Substring(0, length);
        this._offset += Length;
      }
      catch
      {
      }
      return str;
    }

    protected internal string readS()
    {
      string str = "";
      try
      {
        str = Encoding.Unicode.GetString(this._buffer, this._offset, this._buffer.Length - this._offset);
        int length = str.IndexOf(char.MinValue);
        if (length != -1)
          str = str.Substring(0, length);
        this._offset += str.Length + 1;
      }
      catch
      {
      }
      return str;
    }

    protected internal string readUnicode(int Length)
    {
      string str = "";
      try
      {
        str = Encoding.Unicode.GetString(this._buffer, this._offset, Length);
        int length = str.IndexOf(char.MinValue);
        if (length != -1)
          str = str.Substring(0, length);
        this._offset += Length;
      }
      catch
      {
      }
      return str;
    }

    public abstract void read();

    public abstract void run();
  }
}
