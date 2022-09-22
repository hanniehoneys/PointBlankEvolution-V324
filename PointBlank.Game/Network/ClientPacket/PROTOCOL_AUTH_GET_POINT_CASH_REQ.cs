using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_GET_POINT_CASH_REQ : ReceivePacket
  {
    public PROTOCOL_AUTH_GET_POINT_CASH_REQ(GameClient client, byte[] data)
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
        Account player = this._client._player;
        if (player == null)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_GET_POINT_CASH_ACK(0, player._gp, player._money));
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
