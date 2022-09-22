using PointBlank.Battle.Data.Configs;
using SharpDX;
using System;
using System.Text;

namespace PointBlank.Battle.Network
{
  public class ReceivePacket
  {
    private byte[] _buffer;
    private int _offset;

    public ReceivePacket(byte[] buff)
    {
      this._buffer = buff;
    }

    public ReceivePacket(int offset, byte[] buff)
    {
      this._offset = offset;
      this._buffer = buff;
    }

    protected internal byte[] getBuffer()
    {
      return this._buffer;
    }

    protected internal int getOffset()
    {
      return this._offset;
    }

    protected internal void setOffset(int offset)
    {
      this._offset = offset;
    }

    protected internal void Advance(int bytes)
    {
      this._offset += bytes;
      if (this._offset > this._buffer.Length)
      {
        Logger.warning("Advance crashed.");
        throw new Exception("Offset ultrapassou o valor do buffer.");
      }
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

    protected internal Half3 readUHVector()
    {
      return new Half3(this.readUH(), this.readUH(), this.readUH());
    }

    protected internal Half3 readTVector()
    {
      return new Half3(this.readT(), this.readT(), this.readT());
    }

    protected internal byte readC(out bool exception)
    {
      try
      {
        byte num = this._buffer[this._offset++];
        exception = false;
        return num;
      }
      catch
      {
        exception = true;
        return 0;
      }
    }

    protected internal byte readC()
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

    protected internal byte[] readB(int Length)
    {
      try
      {
        byte[] numArray = new byte[Length];
        Array.Copy((Array) this._buffer, this._offset, (Array) numArray, 0, Length);
        this._offset += Length;
        return numArray;
      }
      catch
      {
        return new byte[0];
      }
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

    protected internal float readT()
    {
      float single = BitConverter.ToSingle(this._buffer, this._offset);
      this._offset += 4;
      return single;
    }

    protected internal double readF()
    {
      double num = BitConverter.ToDouble(this._buffer, this._offset);
      this._offset += 8;
      return num;
    }

    protected internal long readQ()
    {
      long int64 = BitConverter.ToInt64(this._buffer, this._offset);
      this._offset += 8;
      return int64;
    }

    protected internal string readS(int Length)
    {
      string str = "";
      try
      {
        str = BattleConfig.EncodeText.GetString(this._buffer, this._offset, Length);
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
  }
}
