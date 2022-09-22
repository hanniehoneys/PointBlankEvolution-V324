using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_TEAM_BALANCE_ACK : SendPacket
  {
    private int _type;
    private int _leader;
    private List<SlotChange> _slots;

    public PROTOCOL_ROOM_TEAM_BALANCE_ACK(List<SlotChange> slots, int leader, int type)
    {
      this._slots = slots;
      this._leader = leader;
      this._type = type;
    }

    public override void write()
    {
      this.writeH((short) 3886);
      this.writeC((byte) this._type);
      this.writeC((byte) this._leader);
      this.writeC((byte) this._slots.Count);
      for (int index = 0; index < this._slots.Count; ++index)
      {
        SlotChange slot = this._slots[index];
        this.writeC((byte) slot.oldSlot._id);
        this.writeC((byte) slot.newSlot._id);
        this.writeC((byte) slot.oldSlot.state);
        this.writeC((byte) slot.newSlot.state);
      }
    }
  }
}
