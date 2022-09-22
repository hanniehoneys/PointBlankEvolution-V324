using System;

namespace PointBlank.Core.Models.Enums
{
  [Flags]
  public enum RoomStageFlag
  {
    NONE = 0,
    RANDOM_MAP = 2,
    PASSWORD = 4,
    OBSERVER_MODE = 8,
    REAL_IP = 16, // 0x00000010
    TEAM_BALANCE = 32, // 0x00000020
    OBSERVER = 64, // 0x00000040
    INTER_ENTER = 128, // 0x00000080
  }
}
