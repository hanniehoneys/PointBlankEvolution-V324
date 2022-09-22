using System;

namespace PointBlank.Battle.Data.Enums
{
  [Flags]
  public enum CHARA_MOVES
  {
    STOPPED = 0,
    LEFT = 1,
    BACK = 2,
    RIGHT = 4,
    FRONT = 8,
    HELI_IN_MOVE = 16, // 0x00000010
    HELI_UNKNOWN = 32, // 0x00000020
    HELI_LEAVE = 64, // 0x00000040
    HELI_STOPPED = 128, // 0x00000080
  }
}
