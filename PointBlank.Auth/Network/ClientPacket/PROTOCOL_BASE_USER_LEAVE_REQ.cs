using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Network;
using System;

namespace PointBlank.Auth.Network.ClientPacket
{
  public class PROTOCOL_BASE_USER_LEAVE_REQ : ReceivePacket
  {
    public PROTOCOL_BASE_USER_LEAVE_REQ(AuthClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      try
      {
        if (this._client == null || this._client._player == null)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_USER_LEAVE_ACK(0));
        this._client.Close(0, false);
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
    }
  }
}
