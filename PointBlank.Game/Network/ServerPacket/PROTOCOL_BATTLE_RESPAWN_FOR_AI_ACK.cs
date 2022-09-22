using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_RESPAWN_FOR_AI_ACK : SendPacket
  {
    private int _slot;

    public PROTOCOL_BATTLE_RESPAWN_FOR_AI_ACK(int slot)
    {
      this._slot = slot;
    }

    public override void write()
    {
      this.writeH((short) 4151);
      this.writeD(this._slot);
    }
  }
}
