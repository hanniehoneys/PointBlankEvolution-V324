using System;
using System.Text;

namespace PointBlank.Core.Network
{
  [Serializable]
  public class BitSet : ICloneable
  {
    private const long serialVersionUID = 7997698588986878753;
    private const int LONG_MASK = 63;
    private long[] bits;

    public BitSet()
      : this(64)
    {
    }

    public BitSet(int nbits)
    {
      if (nbits < 0)
        throw new ArgumentOutOfRangeException("nbits may not be negative");
      uint num = (uint) nbits >> 6;
      if ((uint) (nbits & 63) > 0U)
        ++num;
      this.bits = new long[(int) num];
    }

    public void And(BitSet bs)
    {
      int num = Math.Min(this.bits.Length, bs.bits.Length);
      int index;
      for (index = 0; index < num; ++index)
        this.bits[index] &= bs.bits[index];
      while (index < this.bits.Length)
        this.bits[index++] = 0L;
    }

    public void AndNot(BitSet bs)
    {
      int index = Math.Min(this.bits.Length, bs.bits.Length);
      while (--index >= 0)
        this.bits[index] &= ~bs.bits[index];
    }

    public int Cardinality()
    {
      uint num1 = 0;
      for (int index = this.bits.Length - 1; index >= 0; --index)
      {
        long bit = this.bits[index];
        switch (bit)
        {
          case -1:
            num1 += 64U;
            continue;
          case 0:
            continue;
          default:
            long num2 = (bit >> 1 & 6148914691236517205L) + (bit & 6148914691236517205L);
            long num3 = (num2 >> 2 & 3689348814741910323L) + (num2 & 3689348814741910323L);
            uint num4 = (uint) ((num3 >> 32) + num3);
            uint num5 = (uint) (((int) (num4 >> 4) & 252645135) + ((int) num4 & 252645135));
            uint num6 = (uint) (((int) (num5 >> 8) & 16711935) + ((int) num5 & 16711935));
            num1 += (uint) (((int) (num6 >> 16) & (int) ushort.MaxValue) + ((int) num6 & (int) ushort.MaxValue));
            continue;
        }
      }
      return (int) num1;
    }

    public void Clear(int pos)
    {
      int lastElt = pos >> 6;
      this.Ensure(lastElt);
      this.bits[lastElt] &= ~(1L << pos);
    }

    public void Clear(int from, int to)
    {
      if (from < 0 || from > to)
        throw new ArgumentOutOfRangeException();
      if (from == to)
        return;
      uint num1 = (uint) from >> 6;
      uint num2 = (uint) to >> 6;
      this.Ensure((int) num2);
      if ((int) num1 == (int) num2)
      {
        this.bits[(int) num2] &= (1L << from) - 1L | -1L << to;
      }
      else
      {
        this.bits[(int) num1] &= (1L << from) - 1L;
        this.bits[(int) num2] &= -1L << to;
        for (int index = (int) num1 + 1; (long) index < (long) num2; ++index)
          this.bits[index] = 0L;
      }
    }

    public object Clone()
    {
      try
      {
        BitSet bitSet = ObjectCopier.Clone<BitSet>(this);
        bitSet.bits = (long[]) this.bits.Clone();
        return (object) bitSet;
      }
      catch
      {
        return (object) null;
      }
    }

    public override bool Equals(object obj)
    {
      if (!(obj.GetType() == typeof (BitSet)))
        return false;
      BitSet bitSet = (BitSet) obj;
      int num = Math.Min(this.bits.Length, bitSet.bits.Length);
      int index1;
      for (index1 = 0; index1 < num; ++index1)
      {
        if (this.bits[index1] != bitSet.bits[index1])
          return false;
      }
      for (int index2 = index1; index2 < this.bits.Length; ++index2)
      {
        if ((ulong) this.bits[index2] > 0UL)
          return false;
      }
      for (int index2 = index1; index2 < bitSet.bits.Length; ++index2)
      {
        if ((ulong) bitSet.bits[index2] > 0UL)
          return false;
      }
      return true;
    }

    public void Flip(int index)
    {
      int lastElt = index >> 6;
      this.Ensure(lastElt);
      this.bits[lastElt] ^= 1L << index;
    }

    public void Flip(int from, int to)
    {
      if (from < 0 || from > to)
        throw new ArgumentOutOfRangeException();
      if (from == to)
        return;
      uint num1 = (uint) from >> 6;
      uint num2 = (uint) to >> 6;
      this.Ensure((int) num2);
      if ((int) num1 == (int) num2)
      {
        this.bits[(int) num2] ^= -1L << from & (1L << to) - 1L;
      }
      else
      {
        this.bits[(int) num1] ^= -1L << from;
        this.bits[(int) num2] ^= (1L << to) - 1L;
        for (int index = (int) num1 + 1; (long) index < (long) num2; ++index)
          this.bits[index] ^= -1L;
      }
    }

    public bool Get(int pos)
    {
      int index = pos >> 6;
      if (index >= this.bits.Length)
        return false;
      return (ulong) (this.bits[index] & 1L << pos) > 0UL;
    }

    public BitSet Get(int from, int to)
    {
      if (from < 0 || from > to)
        throw new ArgumentOutOfRangeException();
      BitSet bitSet = new BitSet(to - from);
      uint num1 = (uint) from >> 6;
      if ((long) num1 >= (long) this.bits.Length || to == from)
        return bitSet;
      int num2 = from & 63;
      uint val1 = (uint) to >> 6;
      if (num2 == 0)
      {
        uint num3 = Math.Min((uint) ((int) val1 - (int) num1 + 1), (uint) this.bits.Length - num1);
        Array.Copy((Array) this.bits, (long) num1, (Array) bitSet.bits, 0L, (long) num3);
        if ((long) val1 < (long) this.bits.Length)
          bitSet.bits[(int) val1 - (int) num1] &= (1L << to) - 1L;
        return bitSet;
      }
      uint num4 = Math.Min(val1, (uint) (this.bits.Length - 1));
      int num5 = 64 - num2;
      int index = 0;
      while (num1 < num4)
      {
        bitSet.bits[index] = this.bits[(int) num1] >> num2 | this.bits[(int) num1 + 1] << num5;
        ++num1;
        ++index;
      }
      if ((to & 63) > num2)
        bitSet.bits[index++] = this.bits[(int) num1] >> num2;
      if ((long) val1 < (long) this.bits.Length)
        bitSet.bits[index - 1] &= (1L << to - from) - 1L;
      return bitSet;
    }

    public override int GetHashCode()
    {
      long num = 1234;
      int length = this.bits.Length;
      while (length > 0)
        num ^= (long) length * this.bits[--length];
      return (int) (num >> 32 ^ num);
    }

    public bool Intersects(BitSet set)
    {
      int index = Math.Min(this.bits.Length, set.bits.Length);
      while (--index >= 0)
      {
        if ((ulong) (this.bits[index] & set.bits[index]) > 0UL)
          return true;
      }
      return false;
    }

    public bool IsEmpty()
    {
      for (int index = this.bits.Length - 1; index >= 0; --index)
      {
        if ((ulong) this.bits[index] > 0UL)
          return false;
      }
      return true;
    }

    public int Length
    {
      get
      {
        int index = this.bits.Length - 1;
        while (index >= 0 && this.bits[index] == 0L)
          --index;
        if (index < 0)
          return 0;
        long bit = this.bits[index];
        int num = (index + 1) * 64;
        for (; bit >= 0L; bit <<= 1)
          --num;
        return num;
      }
    }

    public int NextClearBit(int from)
    {
      int index = from >> 6;
      long num = 1L << from;
label_6:
      if (index >= this.bits.Length)
        return from;
      long bit = this.bits[index];
      while ((bit & num) != 0L)
      {
        num <<= 1;
        ++from;
        if ((ulong) num <= 0UL)
        {
          num = 1L;
          ++index;
          goto label_6;
        }
      }
      return from;
    }

    public int NextSetBit(int from)
    {
      int index = from >> 6;
      long num = 1L << from;
label_6:
      if (index >= this.bits.Length)
        return -1;
      long bit = this.bits[index];
      while ((ulong) (bit & num) <= 0UL)
      {
        num <<= 1;
        ++from;
        if ((ulong) num <= 0UL)
        {
          num = 1L;
          ++index;
          goto label_6;
        }
      }
      return from;
    }

    public void Or(BitSet bs)
    {
      this.Ensure(bs.bits.Length - 1);
      for (int index = bs.bits.Length - 1; index >= 0; --index)
        this.bits[index] |= bs.bits[index];
    }

    public void Set(int pos)
    {
      int lastElt = pos >> 6;
      this.Ensure(lastElt);
      this.bits[lastElt] |= 1L << pos;
    }

    public void Set(int index, bool value)
    {
      if (value)
        this.Set(index);
      else
        this.Clear(index);
    }

    public void Set(int from, int to)
    {
      if (from < 0 || from > to)
        throw new ArgumentOutOfRangeException();
      if (from == to)
        return;
      uint num1 = (uint) from >> 6;
      uint num2 = (uint) to >> 6;
      this.Ensure((int) num2);
      if ((int) num1 == (int) num2)
      {
        this.bits[(int) num2] |= -1L << from & (1L << to) - 1L;
      }
      else
      {
        this.bits[(int) num1] |= -1L << from;
        this.bits[(int) num2] |= (1L << to) - 1L;
        for (int index = (int) num1 + 1; (long) index < (long) num2; ++index)
          this.bits[index] = -1L;
      }
    }

    public void Set(int from, int to, bool value)
    {
      if (value)
        this.Set(from, to);
      else
        this.Clear(from, to);
    }

    public int Size
    {
      get
      {
        return this.bits.Length * 64;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder("{");
      bool flag = true;
      for (int index1 = 0; index1 < this.bits.Length; ++index1)
      {
        long num = 1;
        long bit = this.bits[index1];
        if (bit != 0L)
        {
          for (int index2 = 0; index2 < 64; ++index2)
          {
            if ((ulong) (bit & num) > 0UL)
            {
              if (!flag)
                stringBuilder.Append(", ");
              stringBuilder.Append(64 * index1 + index2);
              flag = false;
            }
            num <<= 1;
          }
        }
      }
      return stringBuilder.Append("}").ToString();
    }

    public void XOr(BitSet bs)
    {
      this.Ensure(bs.bits.Length - 1);
      for (int index = bs.bits.Length - 1; index >= 0; --index)
        this.bits[index] ^= bs.bits[index];
    }

    private void Ensure(int lastElt)
    {
      if (lastElt < this.bits.Length)
        return;
      long[] numArray = new long[lastElt + 1];
      Array.Copy((Array) this.bits, 0, (Array) numArray, 0, this.bits.Length);
      this.bits = numArray;
    }

    public bool ContainsAll(BitSet other)
    {
      for (int index = other.bits.Length - 1; index >= 0; --index)
      {
        if ((this.bits[index] & other.bits[index]) != other.bits[index])
          return false;
      }
      return true;
    }
  }
}
