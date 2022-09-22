namespace PointBlank.Core.Models.Enums
{
  public enum RoomType
  {
    None = 0,
    DeathMatch = 4,
    Bomb = 8,
    Destroy = 12, // 0x0000000C
    Annihilation = 16, // 0x00000010
    Defense = 20, // 0x00000014
    FreeForAll = 24, // 0x00000018
    Boss = 28, // 0x0000001C
    Ace = 32, // 0x00000020
    StepUp = 36, // 0x00000024
    Tutorial = 40, // 0x00000028
    Domination = 44, // 0x0000002C
    CrossCounter = 48, // 0x00000030
    Convoy = 52, // 0x00000034
    Runaway = 56, // 0x00000038
  }
}
