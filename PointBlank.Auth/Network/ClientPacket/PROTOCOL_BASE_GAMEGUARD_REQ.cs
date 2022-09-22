using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ClientPacket
{
  public class PROTOCOL_BASE_GAMEGUARD_REQ : ReceivePacket
  {
    private string ClientVersion;

    public PROTOCOL_BASE_GAMEGUARD_REQ(AuthClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.readB(48);
      this.ClientVersion = ((int) this.readC()).ToString() + "." + (object) this.readH();
    }

    public override void run()
    {
      this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GAMEGUARD_ACK());
    }
  }
}
