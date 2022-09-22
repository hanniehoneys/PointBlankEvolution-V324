using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_GET_RECORD_INFO_DB_REQ : ReceivePacket
  {
    private long objId;

    public PROTOCOL_BASE_GET_RECORD_INFO_DB_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.objId = this.readQ();
    }

    public override void run()
    {
      if (this._client._player == null)
        return;
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_RECORD_INFO_DB_ACK(AccountManager.getAccount(this.objId, 0)?._statistic));
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
