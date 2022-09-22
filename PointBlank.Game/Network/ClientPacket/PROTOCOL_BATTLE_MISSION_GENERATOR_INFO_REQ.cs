using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Sync.Client;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_REQ : ReceivePacket
  {
    private List<ushort> damages = new List<ushort>();
    private ushort barRed;
    private ushort barBlue;

    public PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.barRed = this.readUH();
      this.barBlue = this.readUH();
      for (int index = 0; index < 16; ++index)
        this.damages.Add(this.readUH());
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room == null || room.round.Timer != null || (room._state != RoomState.Battle || room.swapRound))
          return;
        PointBlank.Core.Models.Room.Slot slot1 = room.getSlot(player._slotId);
        if (slot1 == null || slot1.state != SlotState.BATTLE)
          return;
        room.Bar1 = (int) this.barRed;
        room.Bar2 = (int) this.barBlue;
        for (int index = 0; index < 16; ++index)
        {
          PointBlank.Core.Models.Room.Slot slot2 = room._slots[index];
          if (slot2._playerId > 0L && slot2.state == SlotState.BATTLE)
          {
            slot2.damageBar1 = this.damages[index];
            slot2.earnedXP = (int) this.damages[index] / 600;
          }
        }
        using (PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_ACK generatorInfoAck = new PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_ACK(room))
          room.SendPacketToPlayers((SendPacket) generatorInfoAck, SlotState.BATTLE, 0);
        if (this.barRed == (ushort) 0)
        {
          RoomSabotageSync.EndRound(room, (byte) 1);
        }
        else
        {
          if (this.barBlue != (ushort) 0)
            return;
          RoomSabotageSync.EndRound(room, (byte) 0);
        }
      }
      catch (Exception ex)
      {
        Logger.info(ex.ToString());
      }
    }
  }
}
