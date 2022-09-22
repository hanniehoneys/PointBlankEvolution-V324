using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_SENDPING_ACK : SendPacket
  {
    private byte[] Pings;

    public PROTOCOL_BATTLE_SENDPING_ACK(byte[] Pings)
    {
      this.Pings = Pings;
    }

    public override void write()
    {
      this.writeH((short) 4123);
      this.writeB(this.Pings);
    }
  }
}
