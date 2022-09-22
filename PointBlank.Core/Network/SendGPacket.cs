using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PointBlank.Core.Network
{
  public class SendGPacket : IDisposable
  {
    public MemoryStream mstream = new MemoryStream();
    private bool disposed = false;
    private SafeHandle handle = (SafeHandle) new SafeFileHandle(IntPtr.Zero, true);

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

    public SendGPacket()
    {
    }

    public SendGPacket(long length)
    {
      this.mstream.SetLength(length);
    }

    public void writeB(byte[] value)
    {
      this.mstream.Write(value, 0, value.Length);
    }

    public void writeB(byte[] value, int offset, int length)
    {
      this.mstream.Write(value, offset, length);
    }

    public void writeD(int value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    public void writeD(uint value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    public void writeH(short value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    public void writeH(ushort value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    public void writeH(int offset, ushort value)
    {
      this.mstream.Position = (long) offset;
      this.writeB(BitConverter.GetBytes(value));
    }

    public void writeC(byte value)
    {
      this.mstream.WriteByte(value);
    }

    public void writeC(int offset, byte value)
    {
      this.mstream.Position = (long) offset;
      this.mstream.WriteByte(value);
    }

    public void writeC(bool value)
    {
      this.mstream.WriteByte(Convert.ToByte(value));
    }

    public void writeF(double value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    public void writeQ(long value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    public void writeQ(ulong value)
    {
      this.writeB(BitConverter.GetBytes(value));
    }

    public void writeS(string value)
    {
      if (value == null)
        return;
      this.writeB(Encoding.Unicode.GetBytes(value));
    }

    public void writeS(string name, int count)
    {
      if (name == null)
        return;
      this.writeB(Config.EncodeText.GetBytes(name));
      this.writeB(new byte[count - name.Length]);
    }

    public void writeS(string name, int count, int CodePage)
    {
      if (name == null)
        return;
      this.writeB(Encoding.GetEncoding(CodePage).GetBytes(name));
      this.writeB(new byte[count - name.Length]);
    }

    public void writeUnicode(string name, int count)
    {
      if (name == null)
        return;
      this.writeB(Encoding.Unicode.GetBytes(name));
      this.writeB(new byte[count - name.Length * 2]);
    }

    public void writeUnicode(string name, bool active)
    {
      if (name == null)
        return;
      this.writeB(Encoding.Unicode.GetBytes(name));
      if (!active)
        return;
      this.writeH((short) 0);
    }
  }
}
