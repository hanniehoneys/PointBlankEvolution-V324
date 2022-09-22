using PointBlank.Core.Models.Enums;

namespace PointBlank.Core.Models.Room
{
  public class Frag
  {
    public byte victimWeaponClass;
    public byte hitspotInfo;
    public byte flag;
    public KillingMessage killFlag;
    public float x;
    public float y;
    public float z;
    public int VictimSlot;
    public int AssistSlot;

    public Frag()
    {
    }

    public Frag(byte hitspotInfo)
    {
      this.SetHitspotInfo(hitspotInfo);
    }

    public void SetHitspotInfo(byte value)
    {
      this.hitspotInfo = value;
      this.VictimSlot = (int) value & 15;
    }
  }
}
