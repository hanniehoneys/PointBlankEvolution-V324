using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_GET_PLAYERINFO_REQ : ReceivePacket
  {
    private int slotId;

    public PROTOCOL_ROOM_GET_PLAYERINFO_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.slotId = this.readD();
    }

    public override void run()
    {
      Account player = this._client._player;
      if (player == null)
        return;
      Room room = player._room;
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_GET_PLAYERINFO_ACK(room?.getPlayerBySlot(this.slotId)));
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
