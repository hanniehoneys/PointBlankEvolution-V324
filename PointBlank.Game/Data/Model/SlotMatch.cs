using PointBlank.Core.Models.Enums;

namespace PointBlank.Game.Data.Model
{
  public class SlotMatch
  {
    public SlotMatchState state;
    public long _playerId;
    public long _id;

    public SlotMatch(int slot)
    {
      this._id = (long) slot;
    }
  }
}
