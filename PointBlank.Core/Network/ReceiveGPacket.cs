using System;
using System.Text;

namespace PointBlank.Core.Network
{
  public class ReceiveGPacket
  {
    private byte[] _buffer;
    private int _offset;

    public ReceiveGPacket(byte[] buff)
    {
      this._buffer = buff;
    }

    public byte[] getBuffer()
    {
      return this._buffer;
    }

    public int readD()
    {
      int int32 = BitConverter.ToInt32(this._buffer, this._offset);
      this._offset += 4;
      return int32;
    }

    public uint readUD()
    {
      uint uint32 = BitConverter.ToUInt32(this._buffer, this._offset);
      this._offset += 4;
      return uint32;
    }

    public byte readC()
    {
      try
      {
        return this._buffer[this._offset++];
      }
      catch
      {
        return 0;
      }
    }

    public byte[] readB(int Length)
    {
      byte[] numArray = new byte[Length];
      Array.Copy((Array) this._buffer, this._offset, (Array) numArray, 0, Length);
      this._offset += Length;
      return numArray;
    }

    public short readH()
    {
      short int16 = BitConverter.ToInt16(this._buffer, this._offset);
      this._offset += 2;
      return int16;
    }

    public ushort readUH()
    {
      ushort uint16 = BitConverter.ToUInt16(this._buffer, this._offset);
      this._offset += 2;
      return uint16;
    }

    public double readF()
    {
      double num = BitConverter.ToDouble(this._buffer, this._offset);
      this._offset += 8;
      return num;
    }

    public float readT()
    {
      float single = BitConverter.ToSingle(this._buffer, this._offset);
      this._offset += 4;
      return single;
    }

    public long readQ()
    {
      long int64 = BitConverter.ToInt64(this._buffer, this._offset);
      this._offset += 8;
      return int64;
    }

    public string readS(int Length)
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

    public string readS(int Length, int CodePage)
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

    public string readS()
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
  }
}
