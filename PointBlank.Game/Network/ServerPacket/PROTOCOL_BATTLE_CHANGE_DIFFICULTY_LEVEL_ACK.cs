using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK : SendPacket
  {
    private Room room;

    public PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK(Room room)
    {
      this.room = room;
    }

    public override void write()
    {
      this.writeH((short) 4149);
      this.writeC(this.room.IngameAiLevel);
      for (int index = 0; index < 16; ++index)
        this.writeD(this.room._slots[index].aiLevel);
    }
  }
}
