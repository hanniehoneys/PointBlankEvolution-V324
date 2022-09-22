using PointBlank.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace PointBlank.Core.Models.Room
{
  public class FragInfos
  {
    public List<Frag> frags = new List<Frag>();
    public byte killerIdx;
    public byte killsCount;
    public byte flag;
    public CharaKillType killingType;
    public int weapon;
    public int Score;
    public float x;
    public float y;
    public float z;

    public KillingMessage GetAllKillFlags()
    {
      KillingMessage killingMessage = (KillingMessage) 0;
      for (int index = 0; index < this.frags.Count; ++index)
      {
        Frag frag = this.frags[index];
        if (!killingMessage.HasFlag((Enum) frag.killFlag))
          killingMessage |= frag.killFlag;
      }
      return killingMessage;
    }
  }
}
