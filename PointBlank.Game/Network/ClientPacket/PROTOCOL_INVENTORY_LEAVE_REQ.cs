using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_INVENTORY_LEAVE_REQ : ReceivePacket
  {
    public PROTOCOL_INVENTORY_LEAVE_REQ(GameClient client, byte[] data)
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
        if (this._client == null)
          return;
        Account player = this._client._player;
        if (player == null)
          return;
        player._room?.changeSlotState(player._slotId, SlotState.NORMAL, true);
        this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_LEAVE_ACK(0));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_INVENTORY_LEAVE_REQ: " + ex.ToString());
      }
    }
  }
}
