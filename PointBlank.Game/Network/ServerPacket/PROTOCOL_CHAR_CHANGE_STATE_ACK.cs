using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CHAR_CHANGE_STATE_ACK : SendPacket
  {
    private Character Character;

    public PROTOCOL_CHAR_CHANGE_STATE_ACK(Character Character)
    {
      this.Character = Character;
    }

    public override void write()
    {
      this.writeH((short) 6153);
      this.writeH((short) 0);
      this.writeD(0);
      this.writeC((byte) 20);
      this.writeC((byte) this.Character.Slot);
    }
  }
}
