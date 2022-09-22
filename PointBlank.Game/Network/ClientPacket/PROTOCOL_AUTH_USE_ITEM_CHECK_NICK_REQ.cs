using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_USE_ITEM_CHECK_NICK_REQ : ReceivePacket
  {
    private string name;

    public PROTOCOL_AUTH_USE_ITEM_CHECK_NICK_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.name = this.readUnicode(66);
    }

    public override void run()
    {
      try
      {
        if (this._client == null || this._client._player == null)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_USE_ITEM_CHECK_NICK_ACK(!PlayerManager.isPlayerNameExist(this.name) ? 0U : 2147483923U));
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
