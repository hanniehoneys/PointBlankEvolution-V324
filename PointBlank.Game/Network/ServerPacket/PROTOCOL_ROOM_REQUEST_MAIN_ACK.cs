using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_ROOM_REQUEST_MAIN_ACK : SendPacket
  {
    private uint _slot;

    public PROTOCOL_ROOM_REQUEST_MAIN_ACK(uint slot)
    {
      this._slot = slot;
    }

    public PROTOCOL_ROOM_REQUEST_MAIN_ACK(int slot)
    {
      this._slot = (uint) slot;
    }

    public override void write()
    {
      this.writeH((short) 3876);
      this.writeD(this._slot);
    }
  }
}
