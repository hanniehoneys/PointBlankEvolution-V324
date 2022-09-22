using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Game.Data.Model;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_LOADING_START_REQ : ReceivePacket
  {
    private string name;

    public PROTOCOL_ROOM_LOADING_START_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.name = this.readS((int) this.readC());
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null)
          return;
        PointBlank.Game.Data.Model.Room room = player._room;
        Slot slot;
        if (room == null || !room.isPreparing() || (!room.getSlot(player._slotId, out slot) || slot.state != SlotState.LOAD))
          return;
        slot.preLoadDate = DateTime.Now;
        room.StartCounter(0, player, slot);
        room.changeSlotState(slot, SlotState.RENDEZVOUS, true);
        room._mapName = this.name;
        if (slot._id != room._leader)
          return;
        room._state = RoomState.Rendezvous;
        room.updateRoomInfo();
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_ROOM_LOADING_START_REQ: " + ex.ToString());
      }
    }
  }
}
