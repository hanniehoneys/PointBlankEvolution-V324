using PointBlank.Battle.Data.Enums;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PointBlank.Battle.Data
{
  public class AllUtils
  {
    public static string gen5(string text)
    {
      using (HMACMD5 hmacmD5 = new HMACMD5(Encoding.UTF8.GetBytes("/x!a@r-$r%an¨.&e&+f*f(f(a)")))
      {
        byte[] hash = hmacmD5.ComputeHash(Encoding.UTF8.GetBytes(text));
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < hash.Length; ++index)
          stringBuilder.Append(hash[index].ToString("x2"));
        return stringBuilder.ToString();
      }
    }

    public static float GetDuration(DateTime date)
    {
      return (float) (DateTime.Now - date).TotalSeconds;
    }

    public static int getIdStatics(int id, int type)
    {
      switch (type)
      {
        case 1:
          return id / 100000;
        case 2:
          return id % 100000 / 1000;
        case 3:
          return id % 1000;
        case 4:
          return id % 10000000 / 100000;
        default:
          return 0;
      }
    }

    public static int CreateItemId(int ItemClass, int ClassType, int Number)
    {
      return ItemClass * 100000 + ClassType * 1000 + Number;
    }

    public static int ItemClass(CLASS_TYPE cw)
    {
      int num1 = 1;
      int num2;
      switch (cw)
      {
        case CLASS_TYPE.Assault:
          num1 = 1;
          goto label_24;
        case CLASS_TYPE.SMG:
          num2 = 1;
          break;
        default:
          num2 = cw == CLASS_TYPE.DualSMG ? 1 : 0;
          break;
      }
      if (num2 != 0)
      {
        num1 = 1;
      }
      else
      {
        int num3;
        switch (cw)
        {
          case CLASS_TYPE.Sniper:
            num1 = 1;
            goto label_24;
          case CLASS_TYPE.Shotgun:
            num3 = 1;
            break;
          default:
            num3 = cw == CLASS_TYPE.DualShotgun ? 1 : 0;
            break;
        }
        if (num3 != 0)
        {
          num1 = 1;
        }
        else
        {
          int num4;
          switch (cw)
          {
            case CLASS_TYPE.HandGun:
            case CLASS_TYPE.DualHandGun:
              num4 = 1;
              break;
            case CLASS_TYPE.MG:
              num1 = 1;
              goto label_24;
            default:
              num4 = cw == CLASS_TYPE.CIC ? 1 : 0;
              break;
          }
          if (num4 != 0)
            num1 = 2;
          else if (cw == CLASS_TYPE.Knife || cw == CLASS_TYPE.DualKnife || cw == CLASS_TYPE.Knuckle)
          {
            num1 = 3;
          }
          else
          {
            switch (cw)
            {
              case CLASS_TYPE.Throwing:
                num1 = 4;
                break;
              case CLASS_TYPE.Item:
                num1 = 5;
                break;
              case CLASS_TYPE.Dino:
                num1 = 0;
                break;
            }
          }
        }
      }
label_24:
      return num1;
    }

    public static OBJECT_TYPE getHitType(uint info)
    {
      return (OBJECT_TYPE) ((int) info & 3);
    }

    public static int getHitWho(uint info)
    {
      return (int) (info >> 2) & 511;
    }

    public static int getHitPart(uint info)
    {
      return (int) (info >> 11) & 63;
    }

    public static ushort getHitDamageBot(uint info)
    {
      return (ushort) (info >> 20);
    }

    public static ushort getHitDamageNormal(uint info)
    {
      return (ushort) (info >> 21);
    }

    public static int getHitHelmet(uint info)
    {
      return (int) (info >> 17) & 7;
    }

    public static int GetRoomInfo(uint UniqueRoomId, int type)
    {
      switch (type)
      {
        case 0:
          return (int) UniqueRoomId & 4095;
        case 1:
          return (int) (UniqueRoomId >> 12) & (int) byte.MaxValue;
        case 2:
          return (int) (UniqueRoomId >> 20) & 4095;
        default:
          return 0;
      }
    }

    public static int GetSeedInfo(uint Seed, int type)
    {
      switch (type)
      {
        case 0:
          return (int) Seed & 4095;
        case 1:
          return (int) (Seed >> 12) & (int) byte.MaxValue;
        case 2:
          return (int) (Seed >> 20) & 4095;
        default:
          return 0;
      }
    }

    public static byte[] Encrypt(byte[] data, int shift)
    {
      byte[] numArray = new byte[data.Length];
      Buffer.BlockCopy((Array) data, 0, (Array) numArray, 0, numArray.Length);
      int length = numArray.Length;
      byte num1 = numArray[0];
      for (int index = 0; index < length; ++index)
      {
        byte num2 = index >= length - 1 ? num1 : numArray[index + 1];
        numArray[index] = (byte) ((int) num2 >> 8 - shift | (int) numArray[index] << shift);
      }
      return numArray;
    }

    public static byte[] Decrypt(byte[] data, int shift)
    {
      try
      {
        byte[] numArray = new byte[data.Length];
        Buffer.BlockCopy((Array) data, 0, (Array) numArray, 0, numArray.Length);
        int length = numArray.Length;
        byte num1 = numArray[length - 1];
        for (int index = length - 1; ((long) index & 2147483648L) == 0L; --index)
        {
          byte num2 = index <= 0 ? num1 : numArray[index - 1];
          numArray[index] = (byte) ((int) num2 << 8 - shift | (int) numArray[index] >> shift);
        }
        return numArray;
      }
      catch
      {
        Logger.warning(BitConverter.ToString(data));
        return new byte[0];
      }
    }

    public static int Percentage(int total, int percent)
    {
      return total * percent / 100;
    }

    public static float Percentage(float total, int percent)
    {
      return (float) ((double) total * (double) percent / 100.0);
    }
  }
}
