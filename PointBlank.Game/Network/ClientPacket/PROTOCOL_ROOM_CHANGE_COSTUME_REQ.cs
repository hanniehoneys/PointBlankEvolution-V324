using PointBlank.Core;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_CHANGE_COSTUME_REQ : ReceivePacket
  {
    private int Team;

    public PROTOCOL_ROOM_CHANGE_COSTUME_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.Team = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        Slot slot = player._room.getSlot(player._slotId);
        if (slot == null && player == null)
          return;
        slot.Costume = this.Team;
        this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_CHANGE_COSTUME_ACK(slot));
      }
      catch (Exception ex)
      {
        Logger.error("PROTOCOL_ROOM_CHANGE_COSTUME_REQ: " + ex.ToString());
      }
    }
  }
}
