using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace PointBlank.Core.Network
{
  public abstract class SendPacket : IDisposable
  {
    public MemoryStream mstream = new MemoryStream();
    private bool disposed = false;
    private SafeHandle handle = (SafeHandle) new SafeFileHandle(IntPtr.Zero, true);

    public byte[] GetBytes(string name)
    {
      try
      {
        this.write();
        return this.mstream.ToArray();
      }
      catch (Exception ex)
      {
        Logger.error("GetBytes problem at: " + name + "\r\n" + ex.ToString());
        return new byte[0];
      }
    }

    public byte[] GetCompleteBytes(string name)
    {
      try
      {
        this.write();
        byte[] array = this.mstream.ToArray();
        if (array.Length < 2)
          return new byte[0];
        ushort uint16 = Convert.ToUInt16(array.Length - 2);
        List<byte> byteList = new List<byte>(array.Length + 2);
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(uint16));
        byteList.AddRange((IEnumerable<byte>) array);
        return byteList.ToArray();
      }
      catch (Exception ex)
      {
        Logger.error("GetCompleteBytes problem at: " + name + "\r\n" + ex.ToString());
        return new byte[0];
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      this.mstream.Dispose();
      if (disposing)
        this.handle.Dispose();
      this.disposed = true;
    }

    protected internal void writeIP(string address)
    {
      this.writeB(IPAddress.Parse(address).GetAddressBytes());
    }

    protected internal void writeIP(IPAddress address)
    {
      this.writeB(address.GetAddressBytes());
    }

    protected internal void writeB(byte[] value)
    {
      this.mstream.Write(value, 0, value.Length);
    }

    protected internal void writeB(byte[] value, int offset, int length)
    {
      this.mstream.Write(value, offset, length);
    }

    protected internal void writeD(bool value)
    {
      this.writeB(new byte[4]
      {
        Convert.ToByte(value),
        (byte) 0,
        (byte) 0,
        (byte) 0
      });
    }

    protected internal void writeD(uint valor)
    {
      this.writeB(BitConverter.GetBytes(valor));
    }

    protected internal void writeD(int value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    protected internal void writeId(int value)
    {
      this.writeB(new byte[4]
      {
        BitConverter.GetBytes(value)[0],
        BitConverter.GetBytes(value)[1],
        BitConverter.GetBytes(value)[2],
        (byte) 64
      });
    }

    protected internal void writeH(ushort valor)
    {
      this.writeB(BitConverter.GetBytes(valor));
    }

    protected internal void writeH(short val)
    {
      this.writeB(BitConverter.GetBytes(val));
    }

    protected internal void writeC(byte value)
    {
      this.mstream.WriteByte(value);
    }

    protected internal void writeC(bool value)
    {
      this.mstream.WriteByte(Convert.ToByte(value));
    }

    protected internal void writeT(float value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    protected internal void writeF(double value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    protected internal void writeQ(ulong valor)
    {
      this.writeB(BitConverter.GetBytes(valor));
    }

    protected internal void writeQ(long valor)
    {
      this.writeB(BitConverter.GetBytes(valor));
    }

    protected internal void writeS(string value)
    {
      if (value == null)
        return;
      this.writeB(Encoding.Unicode.GetBytes(value));
    }

    protected internal void writeS(string name, int count)
    {
      if (name == null)
        return;
      this.writeB(Config.EncodeText.GetBytes(name));
      this.writeB(new byte[count - name.Length]);
    }

    protected internal void writeS(string name, int count, int CodePage)
    {
      if (name == null)
        return;
      this.writeB(Encoding.GetEncoding(CodePage).GetBytes(name));
      this.writeB(new byte[count - name.Length]);
    }

    protected internal void writeUnicode(string name, int count)
    {
      if (name == null)
        return;
      this.writeB(Encoding.Unicode.GetBytes(name));
      this.writeB(new byte[count - name.Length * 2]);
    }

    protected internal void writeUnicode(string name, bool active)
    {
      if (name == null)
        return;
      this.writeB(Encoding.Unicode.GetBytes(name));
      if ((uint) name.Length <= 0U || !active)
        return;
      this.writeH((short) 0);
    }

    protected internal void writeText(string name, int count)
    {
      if (name == null)
        return;
      this.writeB(Encoding.Unicode.GetBytes(name));
      this.writeB(new byte[count - name.Length]);
    }

    public abstract void write();
  }
}
