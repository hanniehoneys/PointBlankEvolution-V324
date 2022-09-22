using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_START_COUNTDOWN_ACK : SendPacket
  {
    private CountDownEnum type;

    public PROTOCOL_BATTLE_START_COUNTDOWN_ACK(CountDownEnum timer)
    {
      this.type = timer;
    }

    public override void write()
    {
      this.writeH((short) 4102);
      this.writeC((byte) this.type);
    }
  }
}
