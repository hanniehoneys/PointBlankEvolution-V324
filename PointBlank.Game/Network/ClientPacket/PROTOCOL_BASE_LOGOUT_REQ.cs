using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_LOGOUT_REQ : ReceivePacket
  {
    public PROTOCOL_BASE_LOGOUT_REQ(GameClient client, byte[] data)
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
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_LOGOUT_ACK());
        this._client.Close(1000, false);
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
        this._client.Close(0, false);
      }
    }
  }
}
