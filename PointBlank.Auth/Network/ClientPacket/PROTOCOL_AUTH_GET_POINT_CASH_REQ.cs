using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ClientPacket
{
  public class PROTOCOL_AUTH_GET_POINT_CASH_REQ : ReceivePacket
  {
    public PROTOCOL_AUTH_GET_POINT_CASH_REQ(AuthClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
    }

    public override void run()
    {
      if (this._client._player == null)
        return;
      this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_GET_POINT_CASH_ACK(0, this._client._player._gp, this._client._player._money));
    }
  }
}
