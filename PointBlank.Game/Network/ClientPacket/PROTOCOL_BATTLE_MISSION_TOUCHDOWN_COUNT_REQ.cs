using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_MISSION_TOUCHDOWN_COUNT_REQ : ReceivePacket
  {
    private int weaponId;
    private int TRex;

    public PROTOCOL_BATTLE_MISSION_TOUCHDOWN_COUNT_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.TRex = (int) this.readC();
      this.weaponId = this.readD();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        if (player == null)
          return;
        PointBlank.Game.Data.Model.Room room = player._room;
        if (room == null || room.round.Timer != null || (room._state != RoomState.Battle || room.TRex != this.TRex))
          return;
        Slot slot = room.getSlot(player._slotId);
        if (slot == null || slot.state != SlotState.BATTLE)
          return;
        if (slot._team == 0)
          room.red_dino += 5;
        else
          room.blue_dino += 5;
        using (PROTOCOL_BATTLE_MISSION_TOUCHDOWN_COUNT_ACK touchdownCountAck = new PROTOCOL_BATTLE_MISSION_TOUCHDOWN_COUNT_ACK(room))
          room.SendPacketToPlayers((SendPacket) touchdownCountAck, SlotState.BATTLE, 0);
        AllUtils.CompleteMission(room, player, slot, MissionType.DEATHBLOW, this.weaponId);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BATTLE_MISSION_TOUCHDOWN_COUNT_REQ: " + ex.ToString());
      }
    }
  }
}
