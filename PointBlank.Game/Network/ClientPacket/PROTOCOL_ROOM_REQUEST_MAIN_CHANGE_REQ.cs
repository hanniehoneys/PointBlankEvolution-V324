using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  internal class PROTOCOL_ROOM_REQUEST_MAIN_CHANGE_REQ : ReceivePacket
  {
    private List<PointBlank.Core.Models.Room.Slot> slots = new List<PointBlank.Core.Models.Room.Slot>();

    public PROTOCOL_ROOM_REQUEST_MAIN_CHANGE_REQ(GameClient client, byte[] data)
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
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room == null || room._leader != player._slotId || room._state != RoomState.Ready)
          return;
        lock (room._slots)
        {
          for (int index = 0; index < 16; ++index)
          {
            PointBlank.Core.Models.Room.Slot slot = room._slots[index];
            if (slot._playerId > 0L && index != room._leader)
              this.slots.Add(slot);
          }
        }
        if (this.slots.Count > 0)
        {
          PointBlank.Core.Models.Room.Slot slot = this.slots[new Random().Next(this.slots.Count)];
          if (room.getPlayerBySlot(slot) != null)
          {
            room.setNewLeader(slot._id, 0, room._leader, false);
            using (PROTOCOL_ROOM_REQUEST_MAIN_CHANGE_ACK requestMainChangeAck = new PROTOCOL_ROOM_REQUEST_MAIN_CHANGE_ACK(slot._id))
              room.SendPacketToPlayers((SendPacket) requestMainChangeAck);
            room.updateSlotsInfo();
          }
          else
            this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_REQUEST_MAIN_CHANGE_ACK(2147483648U));
          this.slots = (List<PointBlank.Core.Models.Room.Slot>) null;
        }
        else
          this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_REQUEST_MAIN_CHANGE_ACK(2147483648U));
      }
      catch (Exception ex)
      {
        Logger.info("ROOM_RANDOM_HOST2_REC: " + ex.ToString());
      }
    }
  }
}
