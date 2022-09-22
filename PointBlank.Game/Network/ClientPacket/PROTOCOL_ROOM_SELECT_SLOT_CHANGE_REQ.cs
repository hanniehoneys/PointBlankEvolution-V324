using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_SELECT_SLOT_CHANGE_REQ : ReceivePacket
  {
    private int TeamIdx;

    public PROTOCOL_ROOM_SELECT_SLOT_CHANGE_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.TeamIdx = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room == null || room.changingSlots)
          return;
        PointBlank.Core.Models.Room.Slot slot = room.getSlot(player._slotId);
        if (slot == null || slot.state != SlotState.NORMAL)
          return;
        Monitor.Enter((object) room._slots);
        room.changingSlots = true;
        List<SlotChange> slots = new List<SlotChange>();
        room.SwitchNewSlot(slots, player, slot, this.TeamIdx, true);
        if (slots.Count > 0)
        {
          using (PROTOCOL_ROOM_TEAM_BALANCE_ACK roomTeamBalanceAck = new PROTOCOL_ROOM_TEAM_BALANCE_ACK(slots, room._leader, 0))
            room.SendPacketToPlayers((SendPacket) roomTeamBalanceAck);
        }
        room.changingSlots = false;
        Monitor.Exit((object) room._slots);
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_ROOM_SELECT_SLOT_CHANGE_REQ: " + ex.ToString());
      }
    }
  }
}
