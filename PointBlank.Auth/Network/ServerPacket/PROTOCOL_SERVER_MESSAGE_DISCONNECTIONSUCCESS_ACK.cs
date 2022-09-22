using PointBlank.Core.Network;
using System;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_SERVER_MESSAGE_DISCONNECTIONSUCCESS_ACK : SendPacket
  {
    private uint _erro;
    private bool type;

    public PROTOCOL_SERVER_MESSAGE_DISCONNECTIONSUCCESS_ACK(uint erro, bool HackUse)
    {
      this._erro = erro;
      this.type = HackUse;
    }

    public override void write()
    {
      this.writeH((short) 2562);
      this.writeD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
      this.writeD(this._erro);
      this.writeD(this.type);
      if (!this.type)
        return;
      this.writeD(0);
    }
  }
}
