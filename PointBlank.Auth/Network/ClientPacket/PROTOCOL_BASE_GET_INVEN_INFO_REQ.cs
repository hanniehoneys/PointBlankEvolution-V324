using PointBlank.Auth.Data.Model;
using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Network;
using System;

namespace PointBlank.Auth.Network.ClientPacket
{
  public class PROTOCOL_BASE_GET_INVEN_INFO_REQ : ReceivePacket
  {
    public PROTOCOL_BASE_GET_INVEN_INFO_REQ(AuthClient client, byte[] data)
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
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_INVEN_INFO_ACK(player._inventory._items));
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_CHARA_INFO_ACK(player));
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_BASE_GET_INVEN_INFO_REQ: " + ex.ToString());
      }
    }
  }
}
