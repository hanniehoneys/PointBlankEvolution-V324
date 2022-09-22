using System;

namespace PointBlank.Core.Models.Enums
{
  [Flags]
  public enum CouponEffects
  {
    Defense90 = 1,
    Ketupat = 2,
    Defense20 = 4,
    HollowPointPlus = 8,
    Defense10 = 16, // 0x00000010
    HP5 = 32, // 0x00000020
    JackHollowPoint = 64, // 0x00000040
    ExtraGrenade = 128, // 0x00000080
    C4SpeedKit = 256, // 0x00000100
    HollowPoint = 512, // 0x00000200
    FullMetalJack = 1024, // 0x00000400
    Defense5 = 2048, // 0x00000800
    Invincible = 4096, // 0x00001000
    HP10 = 8192, // 0x00002000
    QuickChangeReload = 16384, // 0x00004000
    QuickChangeWeapon = 32768, // 0x00008000
    FlashProtect = 65536, // 0x00010000
    GetDroppedWeapon = 131072, // 0x00020000
    Ammo40 = 262144, // 0x00040000
    Respawn20 = 524288, // 0x00080000
    Respawn30 = 1048576, // 0x00100000
    Respawn50 = 2097152, // 0x00200000
    Respawn100 = 4194304, // 0x00400000
    Ammo10 = 8388608, // 0x00800000
    ExtraThrowGrenade = 67108864, // 0x04000000
  }
}
