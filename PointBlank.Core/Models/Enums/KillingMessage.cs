namespace PointBlank.Core.Models.Enums
{
  public enum KillingMessage
  {
    PiercingShot = 1,
    MassKill = 2,
    ChainStopper = 4,
    Headshot = 8,
    ChainHeadshot = 16, // 0x00000010
    ChainSlugger = 32, // 0x00000020
    Suicide = 64, // 0x00000040
    ObjectDefense = 128, // 0x00000080
  }
}
