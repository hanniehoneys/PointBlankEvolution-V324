using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_CHECK_DUPLICATE_REQ : ReceivePacket
  {
    private string clanName;

    public PROTOCOL_CS_CHECK_DUPLICATE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.clanName = this.readUnicode((int) this.readC() * 2);
    }

    public override void run()
    {
      if (this._client == null)
        return;
      if (this._client._player == null)
        return;
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_CS_CHECK_DUPLICATE_ACK(!ClanManager.isClanNameExist(this.clanName) ? 0U : 2147483648U));
      }
      catch
      {
      }
    }
  }
}
