using System;

namespace PointBlank.Core.Models.Enums
{
  [Flags]
  public enum TicketType
  {
    NONE = 0,
    ITEM = 1,
    MONEY = 2,
  }
}
