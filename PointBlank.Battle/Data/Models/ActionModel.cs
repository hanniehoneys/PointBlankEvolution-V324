using PointBlank.Battle.Data.Enums;

namespace PointBlank.Battle.Data.Models
{
  public class ActionModel
  {
    public ushort Slot;
    public ushort Length;
    public UDP_GAME_EVENTS Flag;
    public UDP_SUB_HEAD SubHead;
    public byte[] Data;
  }
}
