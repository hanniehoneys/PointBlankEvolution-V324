using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_NOTIFY_HACK_USER_ACK : SendPacket
  {
    private int slotId;

    public PROTOCOL_BATTLE_NOTIFY_HACK_USER_ACK(int slot)
    {
      this.slotId = slot;
    }

    public override void write()
    {
      this.writeH((short) 3413);
      this.writeC((byte) this.slotId);
      this.writeC((byte) 1);
      this.writeD(1);
    }
  }
}
