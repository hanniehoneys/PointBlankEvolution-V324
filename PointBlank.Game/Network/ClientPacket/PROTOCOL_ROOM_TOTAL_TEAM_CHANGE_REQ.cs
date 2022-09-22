using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_ROOM_TOTAL_TEAM_CHANGE_REQ : ReceivePacket
  {
    private List<SlotChange> changeList = new List<SlotChange>();

    public PROTOCOL_ROOM_TOTAL_TEAM_CHANGE_REQ(GameClient client, byte[] data)
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
        if (room == null || room._leader != player._slotId || (room._state != RoomState.Ready || room.changingSlots))
          return;
        Monitor.Enter((object) room._slots);
        room.changingSlots = true;
        foreach (int oldSlotId in room.RED_TEAM)
        {
          int newSlotId = oldSlotId + 1;
          if (oldSlotId == room._leader)
            room._leader = newSlotId;
          else if (newSlotId == room._leader)
            room._leader = oldSlotId;
          room.SwitchSlots(this.changeList, newSlotId, oldSlotId, true);
        }
        if (this.changeList.Count > 0)
        {
          using (PROTOCOL_ROOM_TEAM_BALANCE_ACK roomTeamBalanceAck = new PROTOCOL_ROOM_TEAM_BALANCE_ACK(this.changeList, room._leader, 2))
          {
            byte[] completeBytes = roomTeamBalanceAck.GetCompleteBytes("PROTOCOL_ROOM_CHANGE_TEAM_REQ");
            for (int index = 0; index < room.getAllPlayers().Count; ++index)
            {
              Account allPlayer = room.getAllPlayers()[index];
              allPlayer._slotId = AllUtils.getNewSlotId(allPlayer._slotId);
              allPlayer.SendCompletePacket(completeBytes);
            }
          }
        }
        room.changingSlots = false;
        Monitor.Exit((object) room._slots);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_ROOM_CHANGE_TEAM_REQ: " + ex.ToString());
      }
    }
  }
}
