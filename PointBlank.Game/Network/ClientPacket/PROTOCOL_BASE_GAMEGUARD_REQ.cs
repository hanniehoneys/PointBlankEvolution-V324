using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_GAMEGUARD_REQ : ReceivePacket
  {
    private byte[] Bytes;
    private string ClientVersion;

    public PROTOCOL_BASE_GAMEGUARD_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.Bytes = this.readB(48);
      this.ClientVersion = ((int) this.readC()).ToString() + "." + (object) this.readH();
    }

    public override void run()
    {
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GAMEGUARD_ACK());
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
