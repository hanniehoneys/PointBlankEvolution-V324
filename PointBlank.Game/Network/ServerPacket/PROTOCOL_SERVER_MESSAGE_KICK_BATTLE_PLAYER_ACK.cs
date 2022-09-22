using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_ACK : SendPacket
  {
    private EventErrorEnum _error;

    public PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_ACK(EventErrorEnum error)
    {
      this._error = error;
    }

    public override void write()
    {
      this.writeH((short) 2564);
      this.writeD((uint) this._error);
    }
  }
}
