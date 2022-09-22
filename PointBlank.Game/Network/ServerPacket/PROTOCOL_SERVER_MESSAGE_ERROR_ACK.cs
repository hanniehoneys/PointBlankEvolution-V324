using PointBlank.Core.Network;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_SERVER_MESSAGE_ERROR_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_SERVER_MESSAGE_ERROR_ACK(uint err)
    {
      this._erro = err;
    }

    public override void write()
    {
      this.writeH((short) 2566);
      this.writeD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
      this.writeD(this._erro);
    }
  }
}
