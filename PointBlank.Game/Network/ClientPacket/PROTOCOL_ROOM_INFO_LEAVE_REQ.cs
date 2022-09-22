using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_INFO_LEAVE_REQ : ReceivePacket
  {
    public PROTOCOL_ROOM_INFO_LEAVE_REQ(GameClient client, byte[] data)
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
        player._room?.changeSlotState(player._slotId, SlotState.NORMAL, true);
        this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_INFO_LEAVE_ACK());
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
